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

namespace SlackNet.AspNetCore;

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
    private readonly ILogger _log;

    public SlackRequestHandler(
        IEnumerable<ISlackRequestListener> requestListeners,
        ISlackHandlerFactory handlerFactory,
        SlackJsonSettings jsonSettings,
        ILogger logger)
    {
        _requestListeners = requestListeners;
        _handlerFactory = handlerFactory;
        _jsonSettings = jsonSettings;
        _log = logger.ForSource<SlackRequestHandler>();
    }

    public Task<SlackResult> HandleEventRequest(HttpRequest request, SlackEndpointConfiguration config) =>
        InRequestContext(request,
            async requestContext =>
                {
                    if (request.Method != "POST")
                    {
                        _log.Internal("Request method {RequestMethod} blocked - only POST is allowed", request.Method);
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);
                    }

                    if (request.ContentType != "application/json")
                    {
                        _log.Internal("Request Content-Type {ContentType} blocked - only application/json is allowed", request.ContentType);
                        return EmptyResult(HttpStatusCode.UnsupportedMediaType);
                    }

                    var requestBody = await ReadString(request).ConfigureAwait(false);
                    var eventRequest = DeserializeEventRequest(requestBody);

                    if (eventRequest is null)
                    {
                        _log.Internal("Unrecognized event request content");
                        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
                    }

                    return EventRequestValidation(requestBody, request.Headers, eventRequest, config)
                        ?? eventRequest switch
                            {
                                EventCallback eventCallback => HandleEvent(requestContext, eventCallback),
                                UrlVerification urlVerification => HandleUrlVerification(urlVerification),
                                _ => UnrecognizedEventRequestType(eventRequest)
                            };
                });

    private SlackResult HandleEvent(SlackRequestContext requestContext, EventCallback eventCallback) =>
        EmptyResult(HttpStatusCode.OK)
            .OnCompleted(() =>
                {
                    var handler = _handlerFactory.CreateEventHandler(requestContext);
                    _log.RequestHandler(handler, eventCallback, "Handling {EventType} event", eventCallback.Event.Type);
                    return handler.Handle(eventCallback);
                });

    private SlackResult HandleUrlVerification(UrlVerification urlVerification)
    {
        _log.Internal("Responding to URL verification challenge");
        return StringResult(HttpStatusCode.OK, urlVerification.Challenge);
    }

    private SlackResult UnrecognizedEventRequestType(EventRequest eventRequest)
    {
        _log.Request("Unrecognized event request type {RequestType}", eventRequest.Type);
        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
    }

    public Task<SlackResult> HandleActionRequest(HttpRequest request, SlackEndpointConfiguration config) =>
        InRequestContext(request,
            async requestContext =>
                {
                    if (request.Method != "POST")
                    {
                        _log.Internal("Request method {RequestMethod} blocked - only POST is allowed", request.Method);
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);
                    }

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var interactionRequest = await DeserializePayload<InteractionRequest>(request).ConfigureAwait(false);

                    if (interactionRequest is null)
                    {
                        _log.Internal("Unrecognized action request content");
                        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
                    }

                    return RequestValidation(await ReadString(request).ConfigureAwait(false), request.Headers, interactionRequest.Token, config)
                        ?? await HandleAction(requestContext, interactionRequest).ConfigureAwait(false);
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
                _ => UnrecognizedInteractionRequestType(interactionRequest)
            };

    private Task<SlackResult> HandleBlockActions(SlackRequestContext requestContext, BlockActionRequest blockActionRequest) =>
        RespondAsync(r =>
            {
                var handler = _handlerFactory.CreateBlockActionHandler(requestContext);
                _log.RequestHandler(handler, blockActionRequest, "Handling {BlockActionType} block action", blockActionRequest.Action.Type);
                return handler.Handle(blockActionRequest, r);
            });

    private async Task<SlackResult> HandleInteractiveMessage(SlackRequestContext requestContext, InteractiveMessage interactiveMessage)
    {
        var handler = _handlerFactory.CreateLegacyInteractiveMessageHandler(requestContext);
        _log.RequestHandler(handler, interactiveMessage, "Handling {InteractiveMessageName} interactive message", interactiveMessage.Action.Name);
        var response = await handler.Handle(interactiveMessage).ConfigureAwait(false);

        var responseObject = response == null ? null
            : interactiveMessage.IsAppUnfurl ? (object)new AttachmentUpdateResponse(response)
            : new MessageUpdateResponse(response);

        return JsonResult(HttpStatusCode.OK, responseObject);
    }

    private async Task<SlackResult> HandleDialogSubmission(SlackRequestContext requestContext, DialogSubmission dialog)
    {
        var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
        _log.RequestHandler(handler, dialog, "Handling dialog submission for {CallbackId}", dialog.CallbackId);
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
        _log.RequestHandler(handler, dialogCancellation, "Handling dialog cancellation for {CallbackId}", dialogCancellation.CallbackId);
        await handler.HandleCancel(dialogCancellation).ConfigureAwait(false);
        return EmptyResult(HttpStatusCode.OK);
    }

    private Task<SlackResult> HandleMessageShortcut(SlackRequestContext requestContext, MessageShortcut messageShortcut) =>
        RespondAsync(r =>
            {
                var handler = _handlerFactory.CreateMessageShortcutHandler(requestContext);
                _log.RequestHandler(handler, messageShortcut, "Handling message shortcut for {CallbackId}", messageShortcut.CallbackId);
                return handler.Handle(messageShortcut, r);
            });

    private Task<SlackResult> HandleGlobalShortcut(SlackRequestContext requestContext, GlobalShortcut globalShortcut) =>
        RespondAsync(r =>
            {
                var handler = _handlerFactory.CreateGlobalShortcutHandler(requestContext);
                _log.RequestHandler(handler, globalShortcut, "Handling global shortcut for {CallbackId}", globalShortcut.CallbackId);
                return handler.Handle(globalShortcut, r);
            });

    private Task<SlackResult> HandleViewSubmission(SlackRequestContext requestContext, ViewSubmission viewSubmission) =>
        RespondAsync<ViewSubmissionResponse>(respond =>
                {
                    var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
                    _log.RequestHandler(handler, viewSubmission, "Handling view submission for {CallbackId}", viewSubmission.View.CallbackId);
                    return handler.Handle(viewSubmission, respond);
                },
            response => response?.ResponseAction == null
                ? EmptyResult(HttpStatusCode.OK)
                : JsonResult(HttpStatusCode.OK, response),
            () => EmptyResult(HttpStatusCode.OK));

    private Task<SlackResult> HandleViewClosed(SlackRequestContext requestContext, ViewClosed viewClosed) =>
        RespondAsync(r =>
            {
                var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
                _log.RequestHandler(handler, viewClosed, "Handling view close for {CallbackId}", viewClosed.View.CallbackId);
                return handler.HandleClose(viewClosed, r);
            });

    private Task<SlackResult> HandleWorkflowStepEdit(SlackRequestContext requestContext, WorkflowStepEdit workflowStepEdit) =>
        RespondAsync(r =>
            {
                var handler = _handlerFactory.CreateWorkflowStepEditHandler(requestContext);
                _log.RequestHandler(handler, workflowStepEdit, "Handling workflow step edit for {CallbackId}", workflowStepEdit.CallbackId);
                return handler.Handle(workflowStepEdit, r);
            });

    private Task<SlackResult> UnrecognizedInteractionRequestType(InteractionRequest interactionRequest)
    {
        _log.Request("Unrecognized interaction request type {RequestType}", interactionRequest.Type);
        return Task.FromResult(StringResult(HttpStatusCode.BadRequest, "Unrecognized content"));
    }

    public Task<SlackResult> HandleOptionsRequest(HttpRequest request, SlackEndpointConfiguration config) =>
        InRequestContext(request,
            async requestContext =>
                {
                    if (request.Method != "POST")
                    {
                        _log.Internal("Request method {RequestMethod} blocked - only POST is allowed", request.Method);
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);
                    }

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var optionsRequest = await DeserializePayload<OptionsRequestBase>(request).ConfigureAwait(false);

                    if (optionsRequest == null)
                    {
                        _log.Internal("Unrecognized options request content");
                        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
                    }

                    return RequestValidation(await ReadString(request).ConfigureAwait(false), request.Headers, optionsRequest.Token, config)
                        ?? optionsRequest switch
                            {
                                OptionsRequest legacyOptionsRequest => await HandleLegacyOptionsRequest(requestContext, legacyOptionsRequest).ConfigureAwait(false),
                                BlockOptionsRequest blockOptionsRequest => await HandleBlockOptionsRequest(requestContext, blockOptionsRequest).ConfigureAwait(false),
                                _ => UnrecognizedOptionsRequestType(optionsRequest)
                            };
                });

    private async Task<SlackResult> HandleLegacyOptionsRequest(SlackRequestContext requestContext, OptionsRequest optionsRequest)
    {
        var handler = _handlerFactory.CreateLegacyOptionProvider(requestContext);
        _log.RequestHandler(handler, optionsRequest, "Handling options request for {RequestName}", optionsRequest.Name);
        var response = await handler.GetOptions(optionsRequest).ConfigureAwait(false);
        return JsonResult(HttpStatusCode.OK, response);
    }

    private async Task<SlackResult> HandleBlockOptionsRequest(SlackRequestContext requestContext, BlockOptionsRequest blockOptionsRequest)
    {
        var handler = _handlerFactory.CreateBlockOptionProvider(requestContext);
        _log.RequestHandler(handler, blockOptionsRequest, "Handling block options request for {ActionId}", blockOptionsRequest.ActionId);
        var response = await handler.GetOptions(blockOptionsRequest).ConfigureAwait(false);
        return JsonResult(HttpStatusCode.OK, response);
    }

    private SlackResult UnrecognizedOptionsRequestType(OptionsRequestBase optionsRequest)
    {
        _log.Request("Unrecognized options request type {RequestType}", optionsRequest.Type);
        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
    }

    public Task<SlackResult> HandleSlashCommandRequest(HttpRequest request, SlackEndpointConfiguration config) =>
        InRequestContext(request,
            async requestContext =>
                {
                    if (request.Method != "POST")
                    {
                        _log.Internal("Request method {RequestMethod} blocked - only POST is allowed", request.Method);
                        return EmptyResult(HttpStatusCode.MethodNotAllowed);
                    }

                    await ReplaceRequestStreamWithMemoryStream(request).ConfigureAwait(false);

                    var command = await DeserializeForm<SlashCommand>(request).ConfigureAwait(false);

                    if (command is null)
                    {
                        _log.Internal("Unrecognized slash command request content");
                        return StringResult(HttpStatusCode.BadRequest, "Unrecognized content");
                    }

                    return RequestValidation(await ReadString(request).ConfigureAwait(false), request.Headers, command.Token, config)
                        ?? await RespondAsync<SlashCommandResponse>(r =>
                                {
                                    var handler = _handlerFactory.CreateSlashCommandHandler(requestContext);
                                    _log.RequestHandler(handler, command, "Handling slash command {SlashCommand}", command.Command);
                                    return handler.Handle(command, r);
                                },
                            response => response == null
                                ? EmptyResult(HttpStatusCode.OK)
                                : JsonResult(HttpStatusCode.OK, new SlashCommandMessageResponse(response)),
                            () => EmptyResult(HttpStatusCode.OK)).ConfigureAwait(false);
                });

    private Task<SlackResult> RespondAsync(Func<Responder, Task> handle) =>
        RespondAsync<int>(r => handle(() => r(0)), _ => EmptyResult(HttpStatusCode.OK), () => EmptyResult(HttpStatusCode.OK));

    private async Task<SlackResult> RespondAsync<T>(Func<Responder<T>, Task> handle, Func<T, SlackResult> buildResult, Func<SlackResult> defaultResult)
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
            return earlyResponse.Task.Result.OnCompleted(async () =>
                {
                    requestComplete.SetResult(0);
                    try
                    {
                        await handlingComplete.ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "Error handling request");
                        throw;
                    }
                });
        }
        else
        {
            await handlingComplete.ConfigureAwait(false); // Fish for exception
            return defaultResult();
        }
    }

    private async Task<SlackResult> InRequestContext(HttpRequest request, Func<SlackRequestContext, Task<SlackResult>> handleRequest)
    {
        var requestContext = SlackRequestContext.Current = new SlackRequestContext
            {
                [nameof(SlackRequestContext.RequestId)] = request.HttpContext.TraceIdentifier
            };
        var requestScope = requestContext.BeginRequest(_requestListeners);

        try
        {
            return (await handleRequest(requestContext).ConfigureAwait(false))
                .OnCompleted(() => requestScope.DisposeAsync().AsTask());
        }
        catch (Exception e)
        {
            _log.Error(e, "Error handling request");
            throw;
        }
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

    private SlackResult EventRequestValidation(string requestBody, IHeaderDictionary headers, EventRequest eventRequest, SlackEndpointConfiguration config) =>
        eventRequest is UrlVerification && !config.VerifyEventUrl
            ? null
            : RequestValidation(requestBody, headers, eventRequest.Token, config);

    private SlackResult RequestValidation(string requestBody, IHeaderDictionary headers, string token, SlackEndpointConfiguration config)
    {
        if (!string.IsNullOrEmpty(config.SigningSecret))
        {
            if (!IsValidSignature(requestBody, headers, config.SigningSecret))
            {
                _log.Internal("Request signature was not signed with the configured signing secret");
                return StringResult(HttpStatusCode.BadRequest, "Invalid signature");
            }
        }
        else
        {
            if (!IsValidToken(token, config.VerificationToken))
            {
                _log.Internal("Request token doesn't match the configured verification token");
                return StringResult(HttpStatusCode.BadRequest, "Invalid token");
            }
        }

        return null;
    }

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