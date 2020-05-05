using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.Events;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public interface ISlackRequestHandler
    {
        Task<SlackResponse> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResponse> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResponse> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResponse> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config);
    }

    class SlackRequestHandler : ISlackRequestHandler
    {
        private readonly IEventHandler _eventHandler;
        private readonly IBlockActionHandler _blockActionHandler;
        private readonly IBlockOptionProvider _blockOptionProvider;
        private readonly IInteractiveMessageHandler _interactiveMessageHandler;
        private readonly IMessageShortcutHandler _messageShortcutHandler;
        private readonly IOptionProvider _optionProvider;
        private readonly IDialogSubmissionHandler _dialogSubmissionHandler;
        private readonly IViewSubmissionHandler _viewSubmissionHandler;
        private readonly ISlashCommandHandler _slashCommandHandler;
        private readonly SlackJsonSettings _jsonSettings;

        public SlackRequestHandler(
            IEventHandler eventHandler,
            IBlockActionHandler blockActionHandler,
            IBlockOptionProvider blockOptionProvider,
            IInteractiveMessageHandler interactiveMessageHandler,
            IMessageShortcutHandler messageShortcutHandler,
            IOptionProvider optionProvider,
            IDialogSubmissionHandler dialogSubmissionHandler,
            IViewSubmissionHandler viewSubmissionHandler,
            ISlashCommandHandler slashCommandHandler,
            SlackJsonSettings jsonSettings)
        {
            _eventHandler = eventHandler;
            _blockActionHandler = blockActionHandler;
            _blockOptionProvider = blockOptionProvider;
            _interactiveMessageHandler = interactiveMessageHandler;
            _messageShortcutHandler = messageShortcutHandler;
            _optionProvider = optionProvider;
            _dialogSubmissionHandler = dialogSubmissionHandler;
            _viewSubmissionHandler = viewSubmissionHandler;
            _slashCommandHandler = slashCommandHandler;
            _jsonSettings = jsonSettings;
        }

        public async Task<SlackResponse> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            if (request.ContentType != "application/json")
                return new EmptyResponse(HttpStatusCode.UnsupportedMediaType);

            var requestBody = await ReadString(request).ConfigureAwait(false);
            var eventRequest = DeserializeEventRequest(requestBody);

            if (!VerifyRequest(requestBody, request.Headers, eventRequest.Token, config))
                return new StringResponse(HttpStatusCode.BadRequest, "Invalid signature/token");

            switch (eventRequest)
            {
                case UrlVerification urlVerification:
                    return new StringResponse(HttpStatusCode.OK, urlVerification.Challenge);

                case EventCallback eventCallback:
                    await _eventHandler.Handle(eventCallback);
                    return new EmptyResponse(HttpStatusCode.OK);

                default:
                    return new StringResponse(HttpStatusCode.BadRequest, "Unrecognized content");
            }
        }

        public async Task<SlackResponse> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

            var interactionRequest = await DeserializePayload<InteractionRequest>(request).ConfigureAwait(false);

            if (interactionRequest != null && VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, interactionRequest.Token, config))
            {
                switch (interactionRequest)
                {
                    case BlockActionRequest blockActions:
                        return await HandleBlockActions(blockActions).ConfigureAwait(false);
                    case InteractiveMessage interactiveMessage:
                        return await HandleInteractiveMessage(interactiveMessage).ConfigureAwait(false);
                    case DialogSubmission dialogSubmission:
                        return await HandleDialogSubmission(dialogSubmission).ConfigureAwait(false);
                    case DialogCancellation dialogCancellation:
                        return await HandleDialogCancellation(dialogCancellation).ConfigureAwait(false);
                    case MessageShortcut messageShortcut:
                        return await HandleMessageShortcut(messageShortcut).ConfigureAwait(false);
                    case ViewSubmission viewSubmission:
                        return await HandleViewSubmission(viewSubmission).ConfigureAwait(false);
                    case ViewClosed viewClosed:
                        return await HandleViewClosed(viewClosed).ConfigureAwait(false);
                }
            }

            return new StringResponse(HttpStatusCode.BadRequest, "Invalid token or unrecognized content");
        }

        private async Task<SlackResponse> HandleBlockActions(BlockActionRequest blockActionRequest)
        {
            await _blockActionHandler.Handle(blockActionRequest).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleInteractiveMessage(InteractiveMessage interactiveMessage)
        {
            var response = await _interactiveMessageHandler.Handle(interactiveMessage).ConfigureAwait(false);

            var responseObject = response == null ? null
                : interactiveMessage.IsAppUnfurl ? (object)new AttachmentUpdateResponse(response)
                : new MessageUpdateResponse(response);

            return new JsonResponse(HttpStatusCode.OK, responseObject);
        }

        private async Task<SlackResponse> HandleDialogSubmission(DialogSubmission dialog)
        {
            var errors = (await _dialogSubmissionHandler.Handle(dialog).ConfigureAwait(false))?.ToList()
                ?? new List<DialogError>();

            var body = new DialogErrorResponse { Errors = errors };
            return errors.Any()
                ? (SlackResponse)new JsonResponse(HttpStatusCode.OK, body)
                : new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleDialogCancellation(DialogCancellation dialogCancellation)
        {
            await _dialogSubmissionHandler.HandleCancel(dialogCancellation).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleMessageShortcut(MessageShortcut messageShortcut)
        {
            await _messageShortcutHandler.Handle(messageShortcut).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleViewSubmission(ViewSubmission viewSubmission)
        {
            var response = await _viewSubmissionHandler.Handle(viewSubmission).ConfigureAwait(false);

            return response?.ResponseAction == null
                ? (SlackResponse)new EmptyResponse(HttpStatusCode.OK)
                : new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<SlackResponse> HandleViewClosed(ViewClosed viewClosed)
        {
            await _viewSubmissionHandler.HandleClose(viewClosed).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        public async Task<SlackResponse> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

            var optionsRequest = await DeserializePayload<OptionsRequestBase>(request).ConfigureAwait(false);

            if (optionsRequest != null && VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, optionsRequest.Token, config))
            {
                switch (optionsRequest)
                {
                    case OptionsRequest legacyOptionsRequest:
                        return await HandleLegacyOptionsRequest(legacyOptionsRequest).ConfigureAwait(false);
                    case BlockOptionsRequest blockOptionsRequest:
                        return await HandleBlockOptionsRequest(blockOptionsRequest).ConfigureAwait(false);
                }
            }

            return new StringResponse(HttpStatusCode.BadRequest, "Invalid token or unrecognized content");
        }

        public async Task<SlackResponse> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

            var command = await DeserializeForm<SlashCommand>(request).ConfigureAwait(false);

            if (!VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, command.Token, config))
                return new StringResponse(HttpStatusCode.BadRequest, "Invalid signature/token");
            
            var response = await _slashCommandHandler.Handle(command).ConfigureAwait(false);

            return response == null 
                ? (SlackResponse)new EmptyResponse(HttpStatusCode.OK) 
                : new JsonResponse(HttpStatusCode.OK, new SlashCommandMessageResponse(response));
        }

        private static async Task ReplaceRequestStreamWithMemoryStream(HttpRequest request)
        {
            var buffer = new MemoryStream();
            await request.Body.CopyToAsync(buffer).ConfigureAwait(false);
            buffer.Seek(0, SeekOrigin.Begin);

            request.Body = buffer;
        }

        private async Task<SlackResponse> HandleLegacyOptionsRequest(OptionsRequest optionsRequest)
        {
            var response = await _optionProvider.GetOptions(optionsRequest).ConfigureAwait(false);
            return new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<SlackResponse> HandleBlockOptionsRequest(BlockOptionsRequest blockOptionsRequest)
        {
            var response = await _blockOptionProvider.GetOptions(blockOptionsRequest).ConfigureAwait(false);
            return new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<T> DeserializePayload<T>(HttpRequest request)
        {
            var form = await request.ReadFormAsync().ConfigureAwait(false);

            return form["payload"]
                .Select(p => JsonConvert.DeserializeObject<T>(p, _jsonSettings.SerializerSettings))
                .FirstOrDefault();
        }

        private async Task<T> DeserializeForm<T>(HttpRequest request)
        {
            var form = await request.ReadFormAsync().ConfigureAwait(false);

            var json = new JObject();
            foreach (var key in form.Keys)
                json[key] = form[key].FirstOrDefault();

            return json.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings));
        }

        private static Task<string> ReadString(HttpRequest request) =>
            new StreamReader(request.Body).ReadToEndAsync();

        private static bool VerifyRequest(string requestBody, IHeaderDictionary headers, string token, SlackEndpointConfiguration config) =>
            !string.IsNullOrEmpty(config.SigningSecret) 
                ? IsValidSignature(requestBody, headers, config.SigningSecret) 
                : IsValidToken(token, config.VerificationToken);

        private static bool IsValidSignature(string requestBody, IHeaderDictionary headers, string signingSecret)
        {
            var encoding = new UTF8Encoding();
            using (var hmac = new HMACSHA256(encoding.GetBytes(signingSecret)))
            {
                var hash = hmac.ComputeHash(encoding.GetBytes($"v0:{headers["X-Slack-Request-Timestamp"]}:{requestBody}"));
                var hashString = $"v0={BitConverter.ToString(hash).Replace("-", "").ToLower()}";

                return hashString.Equals(headers["X-Slack-Signature"]);
            }
        }

        private static bool IsValidToken(string requestToken, string verificationToken) => 
            string.IsNullOrEmpty(verificationToken)
            || requestToken == verificationToken;

        private EventRequest DeserializeEventRequest(string requestBody) =>
            JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<EventRequest>(new JsonTextReader(new StringReader(requestBody)));
    }
}