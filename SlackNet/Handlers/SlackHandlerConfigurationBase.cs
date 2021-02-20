using System;
using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class SlackHandlerConfigurationBase<TConfig> : SlackServiceConfigurationBase<TConfig> where TConfig : SlackHandlerConfigurationBase<TConfig>
    {
        /// <summary>
        /// Take over all event handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceEventHandling(CollectionHandlerFactory<IEventHandler> handlerFactory);

        /// <summary>
        /// Take over all block action handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceBlockActionHandling(CollectionHandlerFactory<IAsyncBlockActionHandler> handlerFactory);

        /// <summary>
        /// Take over all block option providing with your own provider.
        /// </summary>
        public abstract TConfig ReplaceBlockOptionProviding(KeyedHandlerFactory<IBlockOptionProvider> providerFactory);

        /// <summary>
        /// Take over all message shortcut handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceMessageShortcutHandling(CollectionHandlerFactory<IAsyncMessageShortcutHandler> handlerFactory);

        /// <summary>
        /// Take over all global shortcut handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceGlobalShortcutHandling(CollectionHandlerFactory<IAsyncGlobalShortcutHandler> handlerFactory);

        /// <summary>
        /// Take over all view submission handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceViewSubmissionHandling(KeyedHandlerFactory<IAsyncViewSubmissionHandler> handlerFactory);

        /// <summary>
        /// Take over all slash command handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceSlashCommandHandling(KeyedHandlerFactory<IAsyncSlashCommandHandler> handlerFactory);

        /// <summary>
        /// Take over all workflow Step Edit handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceWorkflowStepEditHandling(CollectionHandlerFactory<IAsyncWorkflowStepEditHandler> handlerFactory);

        /// <summary>
        /// Take over all interactive message handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceLegacyInteractiveMessageHandling(KeyedHandlerFactory<IInteractiveMessageHandler> handlerFactory);

        /// <summary>
        /// Take over all legacy option providing with your own provider.
        /// </summary>
        public abstract TConfig ReplaceLegacyOptionProviding(KeyedHandlerFactory<IOptionProvider> providerFactory);

        /// <summary>
        /// Take over all legacy dialog submission handling with your own handler.
        /// </summary>
        public abstract TConfig ReplaceLegacyDialogSubmissionHandling(KeyedHandlerFactory<IDialogSubmissionHandler> handlerFactory);

        public TConfig RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : Event =>
            RegisterEventHandler(handler.ToEventHandler());

        public TConfig RegisterEventHandler(IEventHandler eventHandler) =>
            Chain(() => AddEventHandler(eventHandler));

        public TConfig RegisterBlockActionHandler<TAction>(string actionId, IBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(handler.ToBlockActionHandler(actionId));

        public TConfig RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(handler.ToBlockActionHandler());

        public TConfig RegisterBlockActionHandler(IBlockActionHandler handler) =>
            RegisterAsyncBlockActionHandler(handler.ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, IAsyncBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(handler.ToBlockActionHandler(actionId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(IAsyncBlockActionHandler<TAction> handler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(handler.ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler(IAsyncBlockActionHandler handler) =>
            Chain(() => AddBlockActionHandler(handler));

        public TConfig RegisterBlockOptionProvider(string actionId, IBlockOptionProvider provider) =>
            Chain(() => AddBlockOptionProvider(actionId, provider));

        public TConfig RegisterMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler) =>
            RegisterAsyncMessageShortcutHandler(callbackId, handler.ToMessageShortcutHandler());

        public TConfig RegisterMessageShortcutHandler(IMessageShortcutHandler handler) =>
            RegisterAsyncMessageShortcutHandler(handler.ToMessageShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, IAsyncMessageShortcutHandler handler) =>
            RegisterAsyncMessageShortcutHandler(handler.ToMessageShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(IAsyncMessageShortcutHandler handler) =>
            Chain(() => AddMessageShortcutHandler(handler));

        public TConfig RegisterGlobalShortcutHandler(string callbackId, IGlobalShortcutHandler handler) =>
            RegisterAsyncGlobalShortcutHandler(callbackId, handler.ToGlobalShortcutHandler());

        public TConfig RegisterGlobalShortcutHandler(IGlobalShortcutHandler handler) =>
            RegisterAsyncGlobalShortcutHandler(handler.ToGlobalShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, IAsyncGlobalShortcutHandler handler) =>
            RegisterAsyncGlobalShortcutHandler(handler.ToGlobalShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler) =>
            Chain(() => AddGlobalShortcutHandler(handler));

        public TConfig RegisterViewSubmissionHandler(string callbackId, IViewSubmissionHandler handler) =>
            RegisterAsyncViewSubmissionHandler(callbackId, handler.ToViewSubmissionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler) =>
            Chain(() => AddViewSubmissionHandler(callbackId, handler));

        public TConfig RegisterSlashCommandHandler(string command, ISlashCommandHandler handler) =>
            RegisterAsyncSlashCommandHandler(command, handler.ToSlashCommandHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler(string command, IAsyncSlashCommandHandler handler)
        {
            ValidateCommandName(command);
            return Chain(() => AddSlashCommandHandler(command, handler));
        }

        protected static void ValidateCommandName(string command)
        {
            if (!command.StartsWith("/"))
                throw new ArgumentException("Command must start with '/'", nameof(command));
        }

        public TConfig RegisterWorkflowStepEditHandler(string callbackId, IWorkflowStepEditHandler handler) =>
            RegisterAsyncWorkflowStepEditHandler(callbackId, handler.ToWorkflowStepEditHandler());

        public TConfig RegisterWorkflowStepEditHandler(IWorkflowStepEditHandler handler) =>
            RegisterAsyncWorkflowStepEditHandler(handler.ToWorkflowStepEditHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(string callbackId, IAsyncWorkflowStepEditHandler handler) =>
            RegisterAsyncWorkflowStepEditHandler(handler.ToWorkflowStepEditHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(IAsyncWorkflowStepEditHandler handler) =>
            Chain(() => AddWorkflowStepEditHandler(handler));

        public TConfig RegisterInteractiveMessageHandler(string actionName, IInteractiveMessageHandler handler) =>
            Chain(() => AddLegacyInteractiveMessageHandler(actionName, handler));

        public TConfig RegisterOptionProvider(string actionName, IOptionProvider provider) =>
            Chain(() => AddLegacyOptionProvider(actionName, provider));

        public TConfig RegisterDialogSubmissionHandler(string callbackId, IDialogSubmissionHandler handler) =>
            Chain(() => AddLegacyDialogSubmissionHandler(callbackId, handler));

        protected abstract void AddEventHandler(IEventHandler handler);
        protected abstract void AddBlockActionHandler(IAsyncBlockActionHandler handler);
        protected abstract void AddBlockOptionProvider(string actionId, IBlockOptionProvider provider);
        protected abstract void AddMessageShortcutHandler(IAsyncMessageShortcutHandler handler);
        protected abstract void AddGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler);
        protected abstract void AddViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler);
        protected abstract void AddSlashCommandHandler(string command, IAsyncSlashCommandHandler handler);
        protected abstract void AddWorkflowStepEditHandler(IAsyncWorkflowStepEditHandler handler);

        protected abstract void AddLegacyInteractiveMessageHandler(string actionName, IInteractiveMessageHandler handler);
        protected abstract void AddLegacyOptionProvider(string actionName, IOptionProvider provider);
        protected abstract void AddLegacyDialogSubmissionHandler(string callbackId, IDialogSubmissionHandler handler);
    }

    public delegate THandler CollectionHandlerFactory<THandler>(IEnumerable<THandler> registeredHandlers);
    public delegate THandler KeyedHandlerFactory<THandler>(IHandlerIndex<THandler> registeredHandlers);
}