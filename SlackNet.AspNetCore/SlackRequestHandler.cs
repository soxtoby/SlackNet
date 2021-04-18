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
using SlackNet.Handlers;
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
        private readonly IEnumerable<ISlackRequestListener> _requestListeners;
        private readonly ISlackHandlerFactory _handlerFactory;
        private readonly SlackJsonSettings _jsonSettings;

        public SlackRequestHandler(
            IEnumerable<ISlackRequestListener> requestListeners,
            ISlackHandlerFactory handlerFactory,
            SlackJsonSettings jsonSettings)
        {
            _requestListeners = requestListeners;
            _handlerFactory = handlerFactory;
            _jsonSettings = jsonSettings;
        }

        public Task<SlackResult> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config) =>
            InRequestContext(async requestContext =>
                {
                    if (request.Method != "POST")
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);

                    if (request.ContentType != "application/json")
                        return EmptyResult(HttpStatusCode.UnsupportedMediaType);

                    var requestBody = await ReadString(request).ConfigureAwait(false);
                    var eventRequest = DeserializeEventRequest(requestBody);

                    var shouldSkipVerification = eventRequest is UrlVerification && !config.VerifyEventUrl;
                    var isRequestVerified = shouldSkipVerification || VerifyRequest(requestBody, request.Headers, eventRequest.Token, config);

                    if (!isRequestVerified)
                        return StringResult(HttpStatusCode.BadRequest, "Invalid signature/token");

                    return eventRequest switch
                        {
                            EventCallback eventCallback => HandleEvent(requestContext, eventCallback),
                            UrlVerification urlVerification => StringResult(HttpStatusCode.OK, urlVerification.Challenge),
                            _ => StringResult(HttpStatusCode.BadRequest, "Unrecognized content")
                        };
                });

        private SlackResult HandleEvent(SlackRequestContext requestContext, EventCallback eventCallback) =>
            EmptyResult(HttpStatusCode.OK)
                .OnCompleted(() => _handlerFactory.CreateEventHandler(requestContext).Handle(eventCallback));

        public Task<SlackResult> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config) =>
            InRequestContext(async requestContext =>
                {
                    if (request.Method != "POST")
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var interactionRequest = await DeserializePayload<InteractionRequest>(request).ConfigureAwait(false);

                    if (interactionRequest == null)
                        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");

                    if (!VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, interactionRequest.Token, config))
                        return StringResult(HttpStatusCode.BadRequest, "Invalid token or signing secret");

                    return await HandleAction(requestContext, interactionRequest).ConfigureAwait(false);
                });

        private Task<SlackResult> HandleAction(SlackRequestContext requestContext, InteractionRequest interactionRequest) =>
            interactionRequest switch
                {
                    BlockActionRequest blockActions => HandleBlockActions(requestContext, blockActions),
                    InteractiveMessage interactiveMessage => HandleInteractiveMessage(requestContext, interactiveMessage),
                    DialogSubmission dialogSubmission => HandleDialogSubmission(requestContext, dialogSubmission),
                    DialogCancellation dialogCancellation => HandleDialogCancellation(requestContext, dialogCancellation),
                    MessageShortcut messageShortcut => HandleMessageShortcut(requestContext, messageShortcut),
                    GlobalShortcut globalShortcut => HandleGlobalShortcut(requestContext, globalShortcut),
                    ViewSubmission viewSubmission => HandleViewSubmission(requestContext, viewSubmission),
                    ViewClosed viewClosed => HandleViewClosed(requestContext, viewClosed),
                    WorkflowStepEdit workflowStepEdit => HandleWorkflowStepEdit(requestContext, workflowStepEdit),
                    _ => Task.FromResult(StringResult(HttpStatusCode.BadRequest, "Unrecognized content"))
                };

        private Task<SlackResult> HandleBlockActions(SlackRequestContext requestContext, BlockActionRequest blockActionRequest) =>
            RespondAsync(r => _handlerFactory.CreateBlockActionHandler(requestContext).Handle(blockActionRequest, r));

        private async Task<SlackResult> HandleInteractiveMessage(SlackRequestContext requestContext, InteractiveMessage interactiveMessage)
        {
            var handler = _handlerFactory.CreateLegacyInteractiveMessageHandler(requestContext);
            var response = await handler.Handle(interactiveMessage).ConfigureAwait(false);

            var responseObject = response == null ? null
                : interactiveMessage.IsAppUnfurl ? (object)new AttachmentUpdateResponse(response)
                : new MessageUpdateResponse(response);

            return JsonResult(HttpStatusCode.OK, responseObject);
        }

        private async Task<SlackResult> HandleDialogSubmission(SlackRequestContext requestContext, DialogSubmission dialog)
        {
            var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
            var errors = (await handler.Handle(dialog).ConfigureAwait(false))?.ToList()
                ?? new List<DialogError>();

            var body = new DialogErrorResponse { Errors = errors };
            return errors.Any()
                ? JsonResult(HttpStatusCode.OK, body)
                : EmptyResult(HttpStatusCode.OK);
        }

        private async Task<SlackResult> HandleDialogCancellation(SlackRequestContext requestContext, DialogCancellation dialogCancellation)
        {
            var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
            await handler.HandleCancel(dialogCancellation).ConfigureAwait(false);
            return EmptyResult(HttpStatusCode.OK);
        }

        private Task<SlackResult> HandleMessageShortcut(SlackRequestContext requestContext, MessageShortcut messageShortcut) =>
            RespondAsync(r => _handlerFactory.CreateMessageShortcutHandler(requestContext).Handle(messageShortcut, r));

        private Task<SlackResult> HandleGlobalShortcut(SlackRequestContext requestContext, GlobalShortcut globalShortcut) =>
            RespondAsync(r => _handlerFactory.CreateGlobalShortcutHandler(requestContext).Handle(globalShortcut, r));

        private Task<SlackResult> HandleViewSubmission(SlackRequestContext requestContext, ViewSubmission viewSubmission) =>
            RespondAsync<ViewSubmissionResponse>(respond => _handlerFactory.CreateViewSubmissionHandler(requestContext).Handle(viewSubmission, respond),
                response => response?.ResponseAction == null
                    ? EmptyResult(HttpStatusCode.OK)
                    : JsonResult(HttpStatusCode.OK, response),
                () => EmptyResult(HttpStatusCode.OK));

        private Task<SlackResult> HandleViewClosed(SlackRequestContext requestContext, ViewClosed viewClosed) =>
            RespondAsync(r => _handlerFactory.CreateViewSubmissionHandler(requestContext).HandleClose(viewClosed, r));

        private Task<SlackResult> HandleWorkflowStepEdit(SlackRequestContext requestContext, WorkflowStepEdit workflowStepEdit) =>
            RespondAsync(r => _handlerFactory.CreateWorkflowStepEditHandler(requestContext).Handle(workflowStepEdit, r));

        public Task<SlackResult> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config) =>
            InRequestContext(async requestContext =>
                {
                    if (request.Method != "POST")
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var optionsRequest = await DeserializePayload<OptionsRequestBase>(request).ConfigureAwait(false);

                    if (optionsRequest != null && VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, optionsRequest.Token, config))
                    {
                        switch (optionsRequest)
                        {
                            case OptionsRequest legacyOptionsRequest:
                                return await HandleLegacyOptionsRequest(requestContext, legacyOptionsRequest).ConfigureAwait(false);
                            case BlockOptionsRequest blockOptionsRequest:
                                return await HandleBlockOptionsRequest(requestContext, blockOptionsRequest).ConfigureAwait(false);
                        }
                    }

                    return StringResult(HttpStatusCode.BadRequest, "Invalid token or unrecognized content");
                });

        public Task<SlackResult> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config) =>
            InRequestContext(async requestContext =>
                {
                    if (request.Method != "POST")
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var command = await DeserializeForm<SlashCommand>(request).ConfigureAwait(false);

                    if (!VerifyRequest(await ReadString(request).ConfigureAwait(false), request.Headers, command.Token, config))
                        return StringResult(HttpStatusCode.BadRequest, "Invalid signature/token");

                    return await RespondAsync<SlashCommandResponse>(r => _handlerFactory.CreateSlashCommandHandler(requestContext).Handle(command, r),
                        response => response == null
                            ? EmptyResult(HttpStatusCode.OK)
                            : JsonResult(HttpStatusCode.OK, new SlashCommandMessageResponse(response)),
                        () => EmptyResult(HttpStatusCode.OK)).ConfigureAwait(false);
                });

        private async Task<SlackResult> HandleLegacyOptionsRequest(SlackRequestContext requestContext, OptionsRequest optionsRequest)
        {
            var handler = _handlerFactory.CreateLegacyOptionProvider(requestContext);
            var response = await handler.GetOptions(optionsRequest).ConfigureAwait(false);
            return JsonResult(HttpStatusCode.OK, response);
        }

        private async Task<SlackResult> HandleBlockOptionsRequest(SlackRequestContext requestContext, BlockOptionsRequest blockOptionsRequest)
        {
            var handler = _handlerFactory.CreateBlockOptionProvider(requestContext);
            var response = await handler.GetOptions(blockOptionsRequest).ConfigureAwait(false);
            return JsonResult(HttpStatusCode.OK, response);
        }

        private static Task<SlackResult> RespondAsync(Func<Responder, Task> handle) =>
            RespondAsync<int>(r => handle(() => r(0)), _ => EmptyResult(HttpStatusCode.OK), () => EmptyResult(HttpStatusCode.OK));

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

        private async Task<SlackResult> InRequestContext(Func<SlackRequestContext, Task<SlackResult>> handleRequest)
        {
            var requestContext = new SlackRequestContext();
            var requestScope = requestContext.BeginRequest(_requestListeners);
            return (await handleRequest(requestContext).ConfigureAwait(false))
                .OnCompleted(() => requestScope.DisposeAsync().AsTask());
        }

        private static SlackResult EmptyResult(HttpStatusCode status) =>
            new EmptyResult(status);

        private static SlackResult StringResult(HttpStatusCode status, string body) =>
            new StringResult(status, body);

        private SlackResult JsonResult(HttpStatusCode status, object data) =>
            new JsonResult(_jsonSettings, status, data);

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

        private static Task<string> ReadString(HttpRequest request)
        {
            using var streamReader = new StreamReader(request.Body);
            return streamReader.ReadToEndAsync();
        }

        private static bool VerifyRequest(string requestBody, IHeaderDictionary headers, string token, SlackEndpointConfiguration config) =>
            !string.IsNullOrEmpty(config.SigningSecret)
                ? IsValidSignature(requestBody, headers, config.SigningSecret)
                : IsValidToken(token, config.VerificationToken);

        private static bool IsValidSignature(string requestBody, IHeaderDictionary headers, string signingSecret)
        {
            var encoding = new UTF8Encoding();
            using var hmac = new HMACSHA256(encoding.GetBytes(signingSecret));

            var hash = hmac.ComputeHash(encoding.GetBytes($"v0:{headers["X-Slack-Request-Timestamp"]}:{requestBody}"));
            var hashString = $"v0={BitConverter.ToString(hash).Replace("-", "").ToLower(CultureInfo.InvariantCulture)}";

            return hashString.Equals(headers["X-Slack-Signature"]);
        }

        private static bool IsValidToken(string requestToken, string verificationToken) =>
            string.IsNullOrEmpty(verificationToken)
            || requestToken == verificationToken;

        private EventRequest DeserializeEventRequest(string requestBody)
        {
            using var stringReader = new StringReader(requestBody);
            using var jsonTextReader = new JsonTextReader(stringReader);
            return JsonSerializer.Create(_jsonSettings.SerializerSettings).Deserialize<EventRequest>(jsonTextReader);
        }
    }
}
