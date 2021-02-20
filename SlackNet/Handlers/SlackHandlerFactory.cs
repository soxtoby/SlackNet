using System;
using System.Collections.Generic;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class SlackHandlerFactory : ISlackHandlerFactory
    {
        private const string RequestHandlers = "RequestHandlerInstances";
        private readonly Func<IEventHandler> _eventHandler;
        private readonly Func<IAsyncBlockActionHandler> _blockActionHandler;
        private readonly Func<IBlockOptionProvider> _blockOptionProvider;
        private readonly Func<IAsyncMessageShortcutHandler> _messageShortcutHandler;
        private readonly Func<IAsyncGlobalShortcutHandler> _globalShortcutHandler;
        private readonly Func<IAsyncViewSubmissionHandler> _viewSubmissionHandler;
        private readonly Func<IAsyncSlashCommandHandler> _slashCommandHandler;
        private readonly Func<IAsyncWorkflowStepEditHandler> _workflowStepEditHandler;
        private readonly Func<IInteractiveMessageHandler> _legacyInteractiveMessageHandler;
        private readonly Func<IOptionProvider> _legacyOptionProvider;
        private readonly Func<IDialogSubmissionHandler> _legacyDialogSubmissionHandler;

        public SlackHandlerFactory(
            Func<IEventHandler> eventHandler,
            Func<IAsyncBlockActionHandler> blockActionHandler,
            Func<IBlockOptionProvider> blockOptionProvider,
            Func<IAsyncMessageShortcutHandler> messageShortcutHandler,
            Func<IAsyncGlobalShortcutHandler> globalShortcutHandler,
            Func<IAsyncViewSubmissionHandler> viewSubmissionHandler,
            Func<IAsyncSlashCommandHandler> slashCommandHandler,
            Func<IAsyncWorkflowStepEditHandler> workflowStepEditHandler,
            Func<IInteractiveMessageHandler> legacyInteractiveMessageHandler,
            Func<IOptionProvider> legacyOptionProvider,
            Func<IDialogSubmissionHandler> legacyDialogSubmissionHandler)
        {
            _eventHandler = eventHandler;
            _blockActionHandler = blockActionHandler;
            _blockOptionProvider = blockOptionProvider;
            _messageShortcutHandler = messageShortcutHandler;
            _globalShortcutHandler = globalShortcutHandler;
            _viewSubmissionHandler = viewSubmissionHandler;
            _slashCommandHandler = slashCommandHandler;
            _workflowStepEditHandler = workflowStepEditHandler;
            _legacyInteractiveMessageHandler = legacyInteractiveMessageHandler;
            _legacyOptionProvider = legacyOptionProvider;
            _legacyDialogSubmissionHandler = legacyDialogSubmissionHandler;
        }

        public IEventHandler CreateEventHandler(SlackRequestContext context) => GetOrCreateHandler(context, _eventHandler);
        public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => GetOrCreateHandler(context, _blockActionHandler);
        public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => GetOrCreateHandler(context, _blockOptionProvider);
        public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => GetOrCreateHandler(context, _messageShortcutHandler);
        public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => GetOrCreateHandler(context, _globalShortcutHandler);
        public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => GetOrCreateHandler(context, _viewSubmissionHandler);
        public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => GetOrCreateHandler(context, _slashCommandHandler);
        public IAsyncWorkflowStepEditHandler CreateWorkflowStepEditHandler(SlackRequestContext context) => GetOrCreateHandler(context, _workflowStepEditHandler);
        public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => GetOrCreateHandler(context, _legacyInteractiveMessageHandler);
        public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => GetOrCreateHandler(context, _legacyOptionProvider);
        public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => GetOrCreateHandler(context, _legacyDialogSubmissionHandler);

        private static T GetOrCreateHandler<T>(SlackRequestContext context, Func<T> handlerFactory)
        {
            if (!(context[RequestHandlers] is Dictionary<Type, object> instances))
                context[RequestHandlers] = instances = new Dictionary<Type, object>();

            if (!instances.TryGetValue(typeof(T), out var handler))
                instances[typeof(T)] = handler = handlerFactory();

            return (T)handler;
        }
    }
}