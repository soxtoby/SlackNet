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
        private readonly ISlackEvents _slackEvents;
        private readonly ISlackBlockActions _slackBlockActions;
        private readonly ISlackBlockOptions _slackBlockOptions;
        private readonly ISlackInteractiveMessages _slackInteractiveMessages;
        private readonly ISlackMessageActions _slackMessageActions;
        private readonly ISlackOptions _slackOptions;
        private readonly IDialogSubmissionHandler _dialogSubmissionHandler;
        private readonly ISlackViews _slackViews;
        private readonly ISlackSlashCommands _slackSlashCommands;
        private readonly SlackJsonSettings _jsonSettings;

        public SlackRequestHandler(
            ISlackEvents slackEvents,
            ISlackBlockActions slackBlockActions,
            ISlackBlockOptions slackBlockOptions,
            ISlackInteractiveMessages slackInteractiveMessages,
            ISlackMessageActions slackMessageActions,
            ISlackOptions slackOptions,
            IDialogSubmissionHandler dialogSubmissionHandler,
            ISlackViews slackViews,
            ISlackSlashCommands slackSlashCommands,
            SlackJsonSettings jsonSettings)
        {
            _slackEvents = slackEvents;
            _slackBlockActions = slackBlockActions;
            _slackBlockOptions = slackBlockOptions;
            _slackInteractiveMessages = slackInteractiveMessages;
            _slackMessageActions = slackMessageActions;
            _slackOptions = slackOptions;
            _dialogSubmissionHandler = dialogSubmissionHandler;
            _slackViews = slackViews;
            _slackSlashCommands = slackSlashCommands;
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
                    return new FormUrlEncodedResponse(HttpStatusCode.OK, urlVerification.Challenge);

                case EventCallback eventCallback:
                    _slackEvents.Handle(eventCallback);
                    return new EmptyResponse(HttpStatusCode.OK);

                default:
                    return new StringResponse(HttpStatusCode.BadRequest, "Unrecognized content");
            }
        }

        public async Task<SlackResponse> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            ReplaceRequestStreamWithMemoryStream(request);

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
                    case MessageAction messageAction:
                        return await HandleMessageAction(messageAction).ConfigureAwait(false);
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
            await _slackBlockActions.Handle(blockActionRequest).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleInteractiveMessage(InteractiveMessage interactiveMessage)
        {
            var response = await _slackInteractiveMessages.Handle(interactiveMessage).ConfigureAwait(false);

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

        private async Task<SlackResponse> HandleMessageAction(MessageAction messageAction)
        {
            await _slackMessageActions.Handle(messageAction).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        private async Task<SlackResponse> HandleViewSubmission(ViewSubmission viewSubmission)
        {
            var response = await _slackViews.HandleSubmission(viewSubmission).ConfigureAwait(false);

            return response?.ResponseAction == null
                ? (SlackResponse)new EmptyResponse(HttpStatusCode.OK)
                : new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<SlackResponse> HandleViewClosed(ViewClosed viewClosed)
        {
            await _slackViews.HandleClose(viewClosed).ConfigureAwait(false);
            return new EmptyResponse(HttpStatusCode.OK);
        }

        public async Task<SlackResponse> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResponse(HttpStatusCode.MethodNotAllowed);

            ReplaceRequestStreamWithMemoryStream(request);

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

            ReplaceRequestStreamWithMemoryStream(request);

            var command = await DeserializeForm<SlashCommand>(request).ConfigureAwait(false);

            if (!VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, command.Token, config))
                return new StringResponse(HttpStatusCode.BadRequest, "Invalid signature/token");
            
            var response = await _slackSlashCommands.Handle(command).ConfigureAwait(false);

            return response == null 
                ? (SlackResponse)new EmptyResponse(HttpStatusCode.OK) 
                : new JsonResponse(HttpStatusCode.OK, new SlashCommandMessageResponse(response));
        }

        private static async void ReplaceRequestStreamWithMemoryStream(HttpRequest request)
        {
            var buffer = new MemoryStream();
            await request.Body.CopyToAsync(buffer).ConfigureAwait(false);
            buffer.Seek(0, SeekOrigin.Begin);

            request.Body = buffer;
        }

        private async Task<SlackResponse> HandleLegacyOptionsRequest(OptionsRequest optionsRequest)
        {
            var response = await _slackOptions.Handle(optionsRequest).ConfigureAwait(false);
            return new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<SlackResponse> HandleBlockOptionsRequest(BlockOptionsRequest blockOptionsRequest)
        {
            var response = await _slackBlockOptions.Handle(blockOptionsRequest).ConfigureAwait(false);
            return new JsonResponse(HttpStatusCode.OK, response);
        }

        private async Task<T> DeserializePayload<T>(HttpRequest request)
        {
            var form = await ReadForm(request).ConfigureAwait(false);

            return form["payload"]
                .Select(p => JsonConvert.DeserializeObject<T>(p, _jsonSettings.SerializerSettings))
                .FirstOrDefault();
        }

        private async Task<T> DeserializeForm<T>(HttpRequest request)
        {
            var form = await ReadForm(request).ConfigureAwait(false);

            var json = new JObject();
            foreach (var key in form.Keys)
                json[key] = form[key].FirstOrDefault();

            return json.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings));
        }

        private static async Task<IFormCollection> ReadForm(HttpRequest request)
        {
            var form = await request.ReadFormAsync().ConfigureAwait(false);
            request.Body.Seek(0, SeekOrigin.Begin);
            return form;
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