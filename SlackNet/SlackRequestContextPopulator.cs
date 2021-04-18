using System;
using System.Collections.Generic;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet
{
    class SlackRequestContextPopulator : ISlackRequestListener
    {
        private readonly ISlackServiceProvider _serviceProvider;
        private readonly IReadOnlyCollection<Func<SlackRequestContext, IEventHandler>> _eventHandlers;
        private readonly IReadOnlyCollection<Func<SlackRequestContext, IAsyncBlockActionHandler>> _blockActionHandlers;
        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IBlockOptionProvider>> _blockOptionProviders;
        private readonly IReadOnlyCollection<Func<SlackRequestContext, IAsyncMessageShortcutHandler>> _messageShortcutHandlers;
        private readonly IReadOnlyCollection<Func<SlackRequestContext, IAsyncGlobalShortcutHandler>> _globalShortcutHandlers;
        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IAsyncViewSubmissionHandler>> _viewSubmissionHandlers;
        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IAsyncSlashCommandHandler>> _slashCommandHandlers;
        private readonly IReadOnlyCollection<Func<SlackRequestContext, IAsyncWorkflowStepEditHandler>> _workflowStepEditHandlers;

        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IInteractiveMessageHandler>> _legacyInteractiveMessageHandlers;
        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IOptionProvider>> _legacyOptionProviders;
        private readonly IReadOnlyDictionary<string, Func<SlackRequestContext, IDialogSubmissionHandler>> _legacyDialogSubmissionHandlers;

        public SlackRequestContextPopulator(
            ISlackServiceProvider serviceProvider,
            IReadOnlyCollection<Func<SlackRequestContext, IEventHandler>> eventHandlers,
            IReadOnlyCollection<Func<SlackRequestContext, IAsyncBlockActionHandler>> blockActionHandlers,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IBlockOptionProvider>> blockOptionProviders,
            IReadOnlyCollection<Func<SlackRequestContext, IAsyncMessageShortcutHandler>> messageShortcutHandlers,
            IReadOnlyCollection<Func<SlackRequestContext, IAsyncGlobalShortcutHandler>> globalShortcutHandlers,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IAsyncViewSubmissionHandler>> viewSubmissionHandlers,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IAsyncSlashCommandHandler>> slashCommandHandlers,
            IReadOnlyCollection<Func<SlackRequestContext, IAsyncWorkflowStepEditHandler>> workflowStepEditHandlers,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IInteractiveMessageHandler>> legacyInteractiveMessageHandlers,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IOptionProvider>> legacyOptionProviders,
            IReadOnlyDictionary<string, Func<SlackRequestContext, IDialogSubmissionHandler>> legacyDialogSubmissionHandlers)
        {
            _serviceProvider = serviceProvider;
            _eventHandlers = eventHandlers;
            _blockActionHandlers = blockActionHandlers;
            _blockOptionProviders = blockOptionProviders;
            _messageShortcutHandlers = messageShortcutHandlers;
            _globalShortcutHandlers = globalShortcutHandlers;
            _viewSubmissionHandlers = viewSubmissionHandlers;
            _slashCommandHandlers = slashCommandHandlers;
            _workflowStepEditHandlers = workflowStepEditHandlers;
            _legacyInteractiveMessageHandlers = legacyInteractiveMessageHandlers;
            _legacyOptionProviders = legacyOptionProviders;
            _legacyDialogSubmissionHandlers = legacyDialogSubmissionHandlers;
        }

        public void OnRequestBegin(SlackRequestContext context)
        {
            context[nameof(SlackRequestContext.ServiceProvider)] = _serviceProvider;
            context[nameof(SlackRequestContext.EventHandlers)] = new HandlerCollection<IEventHandler>(context, _eventHandlers);
            context[nameof(SlackRequestContext.BlockActionHandlers)] = new HandlerCollection<IAsyncBlockActionHandler>(context, _blockActionHandlers);
            context[nameof(SlackRequestContext.BlockOptionProviders)] = new HandlerIndex<IBlockOptionProvider>(context, _blockOptionProviders);
            context[nameof(SlackRequestContext.MessageShortcutHandlers)] = new HandlerCollection<IAsyncMessageShortcutHandler>(context, _messageShortcutHandlers);
            context[nameof(SlackRequestContext.GlobalShortcutHandlers)] = new HandlerCollection<IAsyncGlobalShortcutHandler>(context, _globalShortcutHandlers);
            context[nameof(SlackRequestContext.ViewSubmissionHandlers)] = new HandlerIndex<IAsyncViewSubmissionHandler>(context, _viewSubmissionHandlers);
            context[nameof(SlackRequestContext.SlashCommandHandlers)] = new HandlerIndex<IAsyncSlashCommandHandler>(context, _slashCommandHandlers);
            context[nameof(SlackRequestContext.WorkflowStepEditHandlers)] = new HandlerCollection<IAsyncWorkflowStepEditHandler>(context, _workflowStepEditHandlers);
            context[nameof(SlackRequestContext.LegacyInteractiveMessageHandlers)] = new HandlerIndex<IInteractiveMessageHandler>(context, _legacyInteractiveMessageHandlers);
            context[nameof(SlackRequestContext.LegacyOptionProviders)] = new HandlerIndex<IOptionProvider>(context, _legacyOptionProviders);
            context[nameof(SlackRequestContext.LegacyDialogSubmissionHandlers)] = new HandlerIndex<IDialogSubmissionHandler>(context, _legacyDialogSubmissionHandlers);
        }
    }
}