using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;
using WebSocket4Net;

namespace SlackNet
{
    public interface ISlackSocketModeClient : IDisposable
    {
        Task Connect(CancellationToken? cancellationToken = null);

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        bool Connected { get; }
    }

    public class SlackSocketModeClient : ISlackSocketModeClient
    {
        private readonly ICoreSocketModeClient _socket;
        private readonly SlackJsonSettings _jsonSettings;
        private readonly ISlackRequestListener _requestListener;
        private readonly ISlackHandlerFactory _handlerFactory;
        private readonly IDisposable _requestSubscription;

        public SlackSocketModeClient(
            ICoreSocketModeClient socket,
            SlackJsonSettings jsonSettings,
            ISlackRequestListener requestListener,
            ISlackHandlerFactory handlerFactory)
        {
            _socket = socket;
            _jsonSettings = jsonSettings;
            _requestListener = requestListener;
            _handlerFactory = handlerFactory;

            _requestSubscription = _socket.Messages
                .OfType<SocketEnvelope>()
                .SelectMany(HandleRequest)
                .Subscribe();
        }

        public Task Connect(CancellationToken? cancellationToken = null) => _socket.Connect();

        /// <summary>
        /// Is the client connecting or has it connected.
        /// </summary>
        public bool Connected => _socket.Connected;

        private async Task<Unit> HandleRequest(SocketEnvelope envelope)
        {
            try
            {
                var requestContext = new SlackRequestContext();
                await _requestListener.OnRequestBegin(requestContext).ConfigureAwait(false);
                var responded = false;

                try
                {
                    await HandleSpecificRequest(requestContext, envelope, Respond).ConfigureAwait(false);
                }
                finally
                {
                    if (!responded)
                        Respond(new Acknowledgement { EnvelopeId = envelope.EnvelopeId });
                    await _requestListener.OnRequestEnd(requestContext).ConfigureAwait(false);
                }

                void Respond(object payload)
                {
                    responded = true;
                    var ack = payload == null ? new Acknowledgement() : new Acknowledgement<object> { Payload = payload };
                    ack.EnvelopeId = envelope.EnvelopeId;
                    Send(ack);
                }
            }
            catch
            {
                // ignored
            }

            return Unit.Default;
        }

        private Task HandleSpecificRequest(SlackRequestContext requestContext, SocketEnvelope envelope, Action<object> respond) =>
            envelope switch
                {
                    EventEnvelope eventEnvelope => HandleEvent(requestContext, eventEnvelope.Payload, respond),
                    // The "type" properties are identical for InteractiveMessage and OptionsRequest requests, so we need to check for a unique property
                    InteractionEnvelope interaction => interaction.Payload.TryGetValue("action_ts", out _)
                        ? HandleInteraction(requestContext, DeserializePayload<InteractionRequest>(interaction.Payload), respond)
                        : HandleOptionsRequest(requestContext, DeserializePayload<OptionsRequestBase>(interaction.Payload), respond),
                    SlashCommandEnvelope slashCommand => HandleSlashCommand(requestContext, slashCommand.Payload, respond),
                    _ => Task.CompletedTask
                };

        private T DeserializePayload<T>(JObject payload) =>
            payload.ToObject<T>(JsonSerializer.Create(_jsonSettings.SerializerSettings));

        private async Task HandleEvent(SlackRequestContext requestContext, EventCallback eventCallback, Action<object> respond)
        {
            respond(null);
            var handler = _handlerFactory.CreateEventHandler(requestContext);
            await handler.Handle(eventCallback).ConfigureAwait(false);
        }

        private Task HandleInteraction(SlackRequestContext requestContext, InteractionRequest interaction, Action<object> respond) =>
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

        private async Task HandleBlockActions(SlackRequestContext requestContext, BlockActionRequest blockActions, Action<object> respond)
        {
            var handler = _handlerFactory.CreateBlockActionHandler(requestContext);
            await handler.Handle(blockActions, Responder(respond)).ConfigureAwait(false);
        }

        private async Task HandleInteractiveMessage(SlackRequestContext requestContext, InteractiveMessage interactiveMessage, Action<object> respond)
        {
            var handler = _handlerFactory.CreateLegacyInteractiveMessageHandler(requestContext);
            var response = await handler.Handle(interactiveMessage).ConfigureAwait(false);

            if (response == null)
                respond(null);
            else if (interactiveMessage.IsAppUnfurl)
                respond(new AttachmentUpdateResponse(response));
            else
                respond(new MessageUpdateResponse(response));
        }

        private async Task HandleDialogSubmission(SlackRequestContext requestContext, DialogSubmission dialogSubmission, Action<object> respond)
        {
            var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
            var errors = (await handler.Handle(dialogSubmission).ConfigureAwait(false))?.ToList()
                ?? new List<DialogError>();

            if (errors.Any())
                respond(new DialogErrorResponse { Errors = errors });
        }

        private async Task HandleDialogCancellation(SlackRequestContext requestContext, DialogCancellation dialogCancellation)
        {
            var handler = _handlerFactory.CreateLegacyDialogSubmissionHandler(requestContext);
            await handler.HandleCancel(dialogCancellation).ConfigureAwait(false);
        }

        private async Task HandleMessageShortcut(SlackRequestContext requestContext, MessageShortcut messageShortcut, Action<object> respond)
        {
            var handler = _handlerFactory.CreateMessageShortcutHandler(requestContext);
            await handler.Handle(messageShortcut, Responder(respond)).ConfigureAwait(false);
        }

        private async Task HandleGlobalShortcut(SlackRequestContext requestContext, GlobalShortcut globalShortcut, Action<object> respond)
        {
            var handler = _handlerFactory.CreateGlobalShortcutHandler(requestContext);
            await handler.Handle(globalShortcut, Responder(respond)).ConfigureAwait(false);
        }

        private async Task HandleViewSubmission(SlackRequestContext requestContext, ViewSubmission viewSubmission, Action<object> respond)
        {
            var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
            await handler.Handle(viewSubmission,
                response =>
                    {
                        respond(response?.ResponseAction == null ? null : response);
                        return Task.CompletedTask;
                    }).ConfigureAwait(false);
        }

        private async Task HandleViewClosed(SlackRequestContext requestContext, ViewClosed viewClosed, Action<object> respond)
        {
            var handler = _handlerFactory.CreateViewSubmissionHandler(requestContext);
            await handler.HandleClose(viewClosed, Responder(respond)).ConfigureAwait(false);
        }

        private async Task HandleWorkflowStepEdit(SlackRequestContext requestContext, WorkflowStepEdit workflowStepEdit, Action<object> respond)
        {
            var handler = _handlerFactory.CreateWorkflowStepEditHandler(requestContext);
            await handler.Handle(workflowStepEdit, Responder(respond)).ConfigureAwait(false);
        }

        private Task HandleOptionsRequest(SlackRequestContext requestContext, OptionsRequestBase optionsRequest, Action<object> respond) =>
            optionsRequest switch
                {
                    BlockOptionsRequest blockOptionsRequest => HandleBlockOptionsRequest(requestContext, blockOptionsRequest, respond),
                    OptionsRequest legacyOptionsRequest => HandleLegacyOptionsRequest(requestContext, legacyOptionsRequest, respond),
                    _ => Task.CompletedTask
                };

        private async Task HandleBlockOptionsRequest(SlackRequestContext requestContext, BlockOptionsRequest blockOptionsRequest, Action<object> respond)
        {
            var handler = _handlerFactory.CreateBlockOptionProvider(requestContext);
            var blockOptionsResponse = await handler.GetOptions(blockOptionsRequest).ConfigureAwait(false);
            respond(blockOptionsResponse);
        }

        private async Task HandleLegacyOptionsRequest(SlackRequestContext requestContext, OptionsRequest optionsRequest, Action<object> respond)
        {
            var handler = _handlerFactory.CreateLegacyOptionProvider(requestContext);
            var optionsResponse = await handler.GetOptions(optionsRequest).ConfigureAwait(false);
            respond(optionsResponse);
        }

        private async Task HandleSlashCommand(SlackRequestContext requestContext, SlashCommand slashCommand, Action<object> respond)
        {
            var handler = _handlerFactory.CreateSlashCommandHandler(requestContext);
            await handler.Handle(slashCommand,
                response =>
                    {
                        respond(response == null ? null : new SlashCommandMessageResponse(response));
                        return Task.CompletedTask;
                    }).ConfigureAwait(false);
        }

        private static Responder Responder(Action<object> respond) => () =>
            {
                respond(null);
                return Task.CompletedTask;
            };

        private void Send(Acknowledgement acknowledgement) =>
            _socket.Send(acknowledgement);

        public void Dispose()
        {
            _requestSubscription?.Dispose();
        }
    }
}