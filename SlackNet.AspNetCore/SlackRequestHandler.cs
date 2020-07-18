using System;
using System.Collections.Generic;
using System.Globalization;
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
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    public interface ISlackRequestHandler
    {
        Task<SlackResult> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResult> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResult> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config);
        Task<SlackResult> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config);
    }

    class SlackRequestHandler : ISlackRequestHandler
    {
        private readonly IEventHandler _eventHandler;
        private readonly IAsyncBlockActionHandler _blockActionHandler;
        private readonly IBlockOptionProvider _blockOptionProvider;
        private readonly IInteractiveMessageHandler _interactiveMessageHandler;
        private readonly IAsyncMessageShortcutHandler _messageShortcutHandler;
        private readonly IAsyncGlobalShortcutHandler _globalShortcutHandler;
        private readonly IOptionProvider _optionProvider;
        private readonly IDialogSubmissionHandler _dialogSubmissionHandler;
        private readonly IAsyncViewSubmissionHandler _viewSubmissionHandler;
        private readonly IAsyncSlashCommandHandler _slashCommandHandler;
        private readonly SlackJsonSettings _jsonSettings;

        public SlackRequestHandler(
            IEventHandler eventHandler,
            IAsyncBlockActionHandler blockActionHandler,
            IBlockOptionProvider blockOptionProvider,
            IInteractiveMessageHandler interactiveMessageHandler,
            IAsyncMessageShortcutHandler messageShortcutHandler,
            IAsyncGlobalShortcutHandler globalShortcutHandler,
            IOptionProvider optionProvider,
            IDialogSubmissionHandler dialogSubmissionHandler,
            IAsyncViewSubmissionHandler viewSubmissionHandler,
            IAsyncSlashCommandHandler slashCommandHandler,
            SlackJsonSettings jsonSettings)
        {
            _eventHandler = eventHandler;
            _blockActionHandler = blockActionHandler;
            _blockOptionProvider = blockOptionProvider;
            _interactiveMessageHandler = interactiveMessageHandler;
            _messageShortcutHandler = messageShortcutHandler;
            _globalShortcutHandler = globalShortcutHandler;
            _optionProvider = optionProvider;
            _dialogSubmissionHandler = dialogSubmissionHandler;
            _viewSubmissionHandler = viewSubmissionHandler;
            _slashCommandHandler = slashCommandHandler;
            _jsonSettings = jsonSettings;
        }

        public async Task<SlackResult> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResult(HttpStatusCode.MethodNotAllowed);

            if (request.ContentType != "application/json")
                return new EmptyResult(HttpStatusCode.UnsupportedMediaType);

            var requestBody = await ReadString(request).ConfigureAwait(false);
            var eventRequest = DeserializeEventRequest(requestBody);

            var shouldSkipVerification = eventRequest is UrlVerification && !config.VerifyEventUrl;
            var isRequestVerified = shouldSkipVerification || VerifyRequest(requestBody, request.Headers, eventRequest.Token, config);
            
            if (!isRequestVerified)
                return new StringResult(HttpStatusCode.BadRequest, "Invalid signature/token");

            switch (eventRequest)
            {
                case UrlVerification urlVerification:
                    return new StringResult(HttpStatusCode.OK, urlVerification.Challenge);

                case EventCallback eventCallback:
                    return new EmptyResult(HttpStatusCode.OK)
                        .OnCompleted(() => _eventHandler.Handle(eventCallback));

                default:
                    return new StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
            }
        }

        public async Task<SlackResult> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResult(HttpStatusCode.MethodNotAllowed);

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
                    case GlobalShortcut globalShortcut:
                        return await HandleGlobalShortcut(globalShortcut).ConfigureAwait(false);
                    case ViewSubmission viewSubmission:
                        return await HandleViewSubmission(viewSubmission).ConfigureAwait(false);
                    case ViewClosed viewClosed:
                        return await HandleViewClosed(viewClosed).ConfigureAwait(false);
                }
            }

            return new StringResult(HttpStatusCode.BadRequest, "Invalid token or unrecognized content");
        }

        private Task<SlackResult> HandleBlockActions(BlockActionRequest blockActionRequest) =>
            RespondAsync(r => _blockActionHandler.Handle(blockActionRequest, r));

        private async Task<SlackResult> HandleInteractiveMessage(InteractiveMessage interactiveMessage)
        {
            var response = await _interactiveMessageHandler.Handle(interactiveMessage).ConfigureAwait(false);

            var responseObject = response == null ? null
                : interactiveMessage.IsAppUnfurl ? (object)new AttachmentUpdateResponse(response)
                : new MessageUpdateResponse(response);

            return new JsonResult(_jsonSettings, HttpStatusCode.OK, responseObject);
        }

        private async Task<SlackResult> HandleDialogSubmission(DialogSubmission dialog)
        {
            var errors = (await _dialogSubmissionHandler.Handle(dialog).ConfigureAwait(false))?.ToList()
                ?? new List<DialogError>();

            var body = new DialogErrorResponse { Errors = errors };
            return errors.Any()
                ? (SlackResult)new JsonResult(_jsonSettings, HttpStatusCode.OK, body)
                : new EmptyResult(HttpStatusCode.OK);
        }

        private async Task<SlackResult> HandleDialogCancellation(DialogCancellation dialogCancellation)
        {
            await _dialogSubmissionHandler.HandleCancel(dialogCancellation).ConfigureAwait(false);
            return new EmptyResult(HttpStatusCode.OK);
        }

        private Task<SlackResult> HandleMessageShortcut(MessageShortcut messageShortcut) =>
            RespondAsync(r => _messageShortcutHandler.Handle(messageShortcut, r));

        private Task<SlackResult> HandleGlobalShortcut(GlobalShortcut globalShortcut) =>
            RespondAsync(r => _globalShortcutHandler.Handle(globalShortcut, r));

        private Task<SlackResult> HandleViewSubmission(ViewSubmission viewSubmission) =>
            RespondAsync<ViewSubmissionResponse>(
                respond => _viewSubmissionHandler.Handle(viewSubmission, respond),
                response => response?.ResponseAction == null
                    ? (SlackResult)new EmptyResult(HttpStatusCode.OK)
                    : new JsonResult(_jsonSettings, HttpStatusCode.OK, response),
                () => new EmptyResult(HttpStatusCode.OK));

        private Task<SlackResult> HandleViewClosed(ViewClosed viewClosed) =>
            RespondAsync(r => _viewSubmissionHandler.HandleClose(viewClosed, r));

        private static Task<SlackResult> RespondAsync(Func<Responder, Task> handle) => 
            RespondAsync<int>(r => handle(() => r(0)), _ => new EmptyResult(HttpStatusCode.OK), () => new EmptyResult(HttpStatusCode.OK));

        private static async Task<SlackResult> RespondAsync<T>(Func<Responder<T>, Task> handle, Func<T, SlackResult> buildResult, Func<SlackResult> defaultResult)
        {
            var earlyResponse = new TaskCompletionSource<SlackResult>();
            var requestComplete = new TaskCompletionSource<int>();

            var handlingComplete = handle(response =>
            {
                earlyResponse.SetResult(buildResult(response));
                return requestComplete.Task;
            });

            var firstCompletedTask = await Task.WhenAny(earlyResponse.Task, handlingComplete).ConfigureAwait(false);

            if (firstCompletedTask == earlyResponse.Task)
            {
                return earlyResponse.Task.Result.OnCompleted(() =>
                {
                    requestComplete.SetResult(0);
                    return handlingComplete;
                });
            }
            else
            {
                await handlingComplete.ConfigureAwait(false); // Fish for exception
                return defaultResult();
            }
        }

        public async Task<SlackResult> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResult(HttpStatusCode.MethodNotAllowed);

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

            return new StringResult(HttpStatusCode.BadRequest, "Invalid token or unrecognized content");
        }

        public async Task<SlackResult> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config)
        {
            if (request.Method != "POST")
                return new EmptyResult(HttpStatusCode.MethodNotAllowed);

            await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

            var command = await DeserializeForm<SlashCommand>(request).ConfigureAwait(false);

            if (!VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, command.Token, config))
                return new StringResult(HttpStatusCode.BadRequest, "Invalid signature/token");

            return await RespondAsync<SlashCommandResponse>(
                r => _slashCommandHandler.Handle(command, r),
                response => response == null
                    ? (SlackResult)new EmptyResult(HttpStatusCode.OK)
                    : new JsonResult(_jsonSettings, HttpStatusCode.OK, new SlashCommandMessageResponse(response)),
                () => new EmptyResult(HttpStatusCode.OK)).ConfigureAwait(false);
        }

        private async Task<SlackResult> HandleLegacyOptionsRequest(OptionsRequest optionsRequest)
        {
            var response = await _optionProvider.GetOptions(optionsRequest).ConfigureAwait(false);
            return new JsonResult(_jsonSettings, HttpStatusCode.OK, response);
        }

        private async Task<SlackResult> HandleBlockOptionsRequest(BlockOptionsRequest blockOptionsRequest)
        {
            var response = await _blockOptionProvider.GetOptions(blockOptionsRequest).ConfigureAwait(false);
            return new JsonResult(_jsonSettings, HttpStatusCode.OK, response);
        }

        private static async Task ReplaceRequestStreamWithMemoryStream(HttpRequest request)
        {
            var buffer = new MemoryStream();
            await request.Body.CopyToAsync(buffer).ConfigureAwait(false);
            buffer.Seek(0, SeekOrigin.Begin);

            request.Body = buffer;
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
                var hashString = $"v0={BitConverter.ToString(hash).Replace("-", "").ToLower(CultureInfo.InvariantCulture)}";

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