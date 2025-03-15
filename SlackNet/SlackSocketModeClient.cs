using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;

namespace SlackNet;

public interface ISlackSocketModeClient : IDisposable
{
    Task Connect(SocketModeConnectionOptions connectionOptions = null, CancellationToken cancellationToken = default);

    void Disconnect();

    /// <summary>
    /// Is the client connecting or has it connected.
    /// </summary>
    bool Connected { get; }
}

public class SlackSocketModeClient : ISlackSocketModeClient
{
    private const string Envelope = "Envelope";
    private readonly ICoreSocketModeClient _socket;
    private readonly SlackJsonSettings _jsonSettings;
    private readonly IEnumerable<ISlackRequestListener> _requestListeners;
    private readonly ISlackHandlerFactory _handlerFactory;
    private readonly ILogger _log;
    private readonly IDisposable _requestSubscription;

    public SlackSocketModeClient(
        ICoreSocketModeClient socket,
        SlackJsonSettings jsonSettings,
        IEnumerable<ISlackRequestListener> requestListeners,
        ISlackHandlerFactory handlerFactory,
        ILogger logger)
    {
        _socket = socket;
        _jsonSettings = jsonSettings;
        _requestListeners = requestListeners;
        _handlerFactory = handlerFactory;
        _log = logger.ForSource<SlackSocketModeClient>();

        _requestSubscription = _socket.Messages
            .OfType<SocketEnvelope>()
            .SelectMany(HandleRequest)
            .Subscribe();
    }

    public Task Connect(SocketModeConnectionOptions connectionOptions = null, CancellationToken cancellationToken = default) =>
        _socket.Connect(connectionOptions, cancellationToken);

    public void Disconnect() => _socket.Disconnect();

    /// <summary>
    /// Is the client connecting or has it connected.
    /// </summary>
    public bool Connected => _socket.Connected;

    private async Task<Unit> HandleRequest(SocketEnvelope envelope)
    {
        try
        {
            var requestContext = SlackRequestContext.Current = new SlackRequestContext
                {
                    [Envelope] = envelope,
                    [nameof(SlackRequestContext.RequestId)] = envelope.RequestId
                };

            await using (requestContext.BeginRequest(_requestListeners).ConfigureAwait(false))
            {
                var responded = false;

                try
                {
                    await HandleSpecificRequest(requestContext, envelope, Respond).ConfigureAwait(false);
                }
                finally
                {
                    if (!responded)
                        await Respond(null).ConfigureAwait(false);
                }

                async Task Respond(object payload)
                {
                    responded = true;
                    Acknowledgement ack;
                    if (payload == null)
                    {
                        ack = new Acknowledgement();
                        _log.Request("Acknowledging request");
                    }
                    else
                    {
                        ack = new Acknowledgement<object> { Payload = payload };
                        _log.WithContext("Payload", payload)
                            .Request("Responding with {PayloadType}", payload.GetType());
                    }
                    ack.EnvelopeId = envelope.EnvelopeId;
                    await Send(envelope.SocketId, ack).ConfigureAwait(false);
                }
            }
        }
        catch (Exception e)
        {
            _log.Error(e, "Error handling request");
        }

        return Unit.Default;
    }

    private Task HandleSpecificRequest(SlackRequestContext requestContext, SocketEnvelope envelope, Func<object, Task> respond) =>
        envelope switch
            {
                EventEnvelope eventEnvelope => HandleEvent(requestContext, eventEnvelope.Payload, respond),
                SlashCommandEnvelope slashCommand => HandleSlashCommand(requestContext, slashCommand.Payload, respond),
                InteractionEnvelope interaction when IsBlockOptionsRequest(interaction) => HandleBlockOptionsRequest(requestContext, DeserializePayload<BlockOptionsRequest>(interaction.Payload), respond),
                InteractionEnvelope interaction when IsLegacyOptionsRequest(interaction) => HandleLegacyOptionsRequest(requestContext, DeserializePayload<OptionsRequest>(interaction.Payload), respond),
                InteractionEnvelope interaction => HandleInteraction(requestContext, DeserializePayload<InteractionRequest>(interaction.Payload), respond),
                _ => Task.CompletedTask
            };

    private static bool IsBlockOptionsRequest(InteractionEnvelope interaction) =>
        interaction.Payload.Value<string>("type") == typeof(BlockOptionsRequest).GetTypeInfo().SlackType();

    // The "type" properties are identical for InteractiveMessage and OptionsRequest requests, so we need to check for a unique property
    private static bool IsLegacyOptionsRequest(InteractionEnvelope interaction) =>
        interaction.Payload.Value<string>("type") == typeof(InteractiveMessage).GetTypeInfo().SlackType()
        && !interaction.Payload.TryGetValue("response_url", out _);

    private T DeserializePayload<T>(JObject payload) =>
        payload.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings));

    private async Task HandleEvent(SlackRequestContext requestContext, EventCallback eventCallback, Func<object, Task> respond)
    {
        await respond(null).ConfigureAwait(false);
        var handler = _handlerFactory.CreateEventHandler(requestContext);
        _log.RequestHandler(handler, eventCallback, "Handling {EventType} event", eventCallback.Event.Type);
        await handler.Handle(eventCallback).ConfigureAwait(false);
    }

    private Task HandleInteraction(SlackRequestContext requestContext, InteractionRequest interaction, Func<object, Task> respond) =>
        interaction switch
            {
                BlockActionRequest blockActions => HandleBlockActions(requestContext, blockActions, respond),
                InteractiveMessage interactiveMessage => HandleInteractiveMessage(requestContext, interactiveMessage, respond),
                DialogSubmission dialogSubmission => HandleDialogSubmission(requestContext, dialogSubmission, respond),
                DialogCancellation dialogCancellation => HandleDialogCancellation(requestContext, dialogCancellation),
                MessageShortcut messageShortcut => HandleMessageShortcut(requestContext, messageShortcut, respond),
                GlobalShortcut globalShortcut => HandleGlobalShortcut(requestContext, globalShortcut, respond),
                ViewSubmission viewSubmission => HandleViewSubmission(requestContext, viewSubmission, respond),
                ViewClosed viewClosed => HandleViewClosed(requestContext, viewClosed, respond),
                WorkflowStepEdit workflowStepEdit => HandleWorkflowStepEdit(requestContext, workflowStepEdit, respond),
                _ => Task.CompletedTask
            };

    private async Task HandleBlockActions(SlackRequestContext requestContext, BlockActionRequest blockActions, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateBlockActionHandler(requestContext);
        _log.RequestHandler(handler, blockActions, "Handling {BlockActionType} block action", blockActions.Action.Type);
        await handler.Handle(blockActions, Responder(respond)).ConfigureAwait(false);
    }

    private async Task HandleInteractiveMessage(SlackRequestContext requestContext, InteractiveMessage interactiveMessage, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateLegacyInteractiveMessageHandler(requestContext);
        _log.RequestHandler(handler, interactiveMessage, "Handling {InteractiveMessageName} interactive message", interactiveMessage.Action.Name);
        var response = await handler.Handle(interactiveMessage).ConfigureAwait(false);

        if (response == null)
            await respond(null).ConfigureAwait(false);
        else if (interactiveMessage.IsAppUnfurl)
            await respond(new AttachmentUpdateResponse(response)).ConfigureAwait(false);
        else
            await respond(new MessageUpdateResponse(response)).ConfigureAwait(false);
    }

    private async Task HandleDialogSubmission(SlackRequestContext requestContext, DialogSubmission dialogSubmission, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
        _log.RequestHandler(handler, dialogSubmission, "Handling dialog submission for {CallbackId}", dialogSubmission.CallbackId);
        var errors = (await handler.Handle(dialogSubmission).ConfigureAwait(false))?.ToList()
            ?? [];

        if (errors.Any())
            await respond(new DialogErrorResponse { Errors = errors }).ConfigureAwait(false);
    }

    private async Task HandleDialogCancellation(SlackRequestContext requestContext, DialogCancellation dialogCancellation)
    {
        var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
        _log.RequestHandler(handler, dialogCancellation, "Handling dialog cancellation for {CallbackId}", dialogCancellation.CallbackId);
        await handler.HandleCancel(dialogCancellation).ConfigureAwait(false);
    }

    private async Task HandleMessageShortcut(SlackRequestContext requestContext, MessageShortcut messageShortcut, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateMessageShortcutHandler(requestContext);
        _log.RequestHandler(handler, messageShortcut, "Handling message shortcut for {CallbackId}", messageShortcut.CallbackId);
        await handler.Handle(messageShortcut, Responder(respond)).ConfigureAwait(false);
    }

    private async Task HandleGlobalShortcut(SlackRequestContext requestContext, GlobalShortcut globalShortcut, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateGlobalShortcutHandler(requestContext);
        _log.RequestHandler(handler, globalShortcut, "Handling global shortcut for {CallbackId}", globalShortcut.CallbackId);
        await handler.Handle(globalShortcut, Responder(respond)).ConfigureAwait(false);
    }

    private async Task HandleViewSubmission(SlackRequestContext requestContext, ViewSubmission viewSubmission, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
        _log.RequestHandler(handler, viewSubmission, "Handling view submission for {CallbackId}", viewSubmission.View.CallbackId);
        await handler.Handle(viewSubmission,
            response =>
                {
                    respond(response?.ResponseAction == null ? null : response);
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
    }

    private async Task HandleViewClosed(SlackRequestContext requestContext, ViewClosed viewClosed, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
        _log.RequestHandler(handler, viewClosed, "Handling view close for {CallbackId}", viewClosed.View.CallbackId);
        await handler.HandleClose(viewClosed, Responder(respond)).ConfigureAwait(false);
    }

    private async Task HandleWorkflowStepEdit(SlackRequestContext requestContext, WorkflowStepEdit workflowStepEdit, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateWorkflowStepEditHandler(requestContext);
        _log.RequestHandler(handler, workflowStepEdit, "Handling workflow step edit for {CallbackId}", workflowStepEdit.CallbackId);
        await handler.Handle(workflowStepEdit, Responder(respond)).ConfigureAwait(false);
    }

    private async Task HandleBlockOptionsRequest(SlackRequestContext requestContext, BlockOptionsRequest blockOptionsRequest, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateBlockOptionProvider(requestContext);
        _log.RequestHandler(handler, blockOptionsRequest, "Handling block options request for {ActionId}", blockOptionsRequest.ActionId);
        var blockOptionsResponse = await handler.GetOptions(blockOptionsRequest).ConfigureAwait(false);
        await respond(blockOptionsResponse).ConfigureAwait(false);
    }

    private async Task HandleLegacyOptionsRequest(SlackRequestContext requestContext, OptionsRequest optionsRequest, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateLegacyOptionProvider(requestContext);
        _log.RequestHandler(handler, optionsRequest, "Handling options request for {RequestName}", optionsRequest.Name);
        var optionsResponse = await handler.GetOptions(optionsRequest).ConfigureAwait(false);
        await respond(optionsResponse).ConfigureAwait(false);
    }

    private async Task HandleSlashCommand(SlackRequestContext requestContext, SlashCommand slashCommand, Func<object, Task> respond)
    {
        var handler = _handlerFactory.CreateSlashCommandHandler(requestContext);
        _log.RequestHandler(handler, slashCommand, "Handling slash command {SlashCommand}", slashCommand.Command);
        await handler.Handle(slashCommand,
            response =>
                {
                    respond(response == null ? null : new SlashCommandMessageResponse(response));
                    return Task.CompletedTask;
                }).ConfigureAwait(false);
    }

    private static Responder Responder(Func<object, Task> respond) => () => respond(null);

    private Task Send(int socketId, Acknowledgement acknowledgement) =>
        _socket.Send(socketId, acknowledgement);

    public void Dispose()
    {
        _requestSubscription?.Dispose();
    }
}
