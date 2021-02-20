using System;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class FactorySlackHandlerConfigurationWithExternalDependencyResolver<TConfig> : FactorySlackHandlerConfiguration<TConfig> where TConfig : FactorySlackHandlerConfigurationWithExternalDependencyResolver<TConfig>
    {
        public TConfig RegisterEventHandler<TEvent>(Func<IEventHandler<TEvent>> handlerFactory) where TEvent : Event =>
            RegisterEventHandler(() => handlerFactory().ToEventHandler());

        public TConfig RegisterEventHandler(Func<IEventHandler> handlerFactory) =>
            Chain(() => AddEventHandler(handlerFactory));

        public TConfig RegisterBlockActionHandler<TAction>(string actionId, Func<IBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(() => handlerFactory().ToBlockActionHandler(actionId));

        public TConfig RegisterBlockActionHandler<TAction>(Func<IBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(() => handlerFactory().ToBlockActionHandler());

        public TConfig RegisterBlockActionHandler(Func<IBlockActionHandler> handlerFactory) =>
            RegisterAsyncBlockActionHandler(() => handlerFactory().ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<IAsyncBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(() => handlerFactory().ToBlockActionHandler(actionId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(Func<IAsyncBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(() => handlerFactory().ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler(Func<IAsyncBlockActionHandler> handlerFactory) =>
            Chain(() => AddBlockActionHandler(handlerFactory));

        public TConfig RegisterBlockOptionProvider(string actionId, Func<IBlockOptionProvider> providerFactory) =>
            Chain(() => AddBlockOptionProvider(actionId, providerFactory));

        public TConfig RegisterMessageShortcutHandler(string callbackId, Func<IMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(callbackId, () => handlerFactory().ToMessageShortcutHandler());

        public TConfig RegisterMessageShortcutHandler(Func<IMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(() => handlerFactory().ToMessageShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, Func<IAsyncMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(() => handlerFactory().ToMessageShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(Func<IAsyncMessageShortcutHandler> handlerFactory) =>
            Chain(() => AddMessageShortcutHandler(handlerFactory));

        public TConfig RegisterGlobalShortcutHandler(string callbackId, Func<IGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(callbackId, () => handlerFactory().ToGlobalShortcutHandler());

        public TConfig RegisterGlobalShortcutHandler(Func<IGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(() => handlerFactory().ToGlobalShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, Func<IAsyncGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(() => handlerFactory().ToGlobalShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(Func<IAsyncGlobalShortcutHandler> handlerFactory) =>
            Chain(() => AddGlobalShortcutHandler(handlerFactory));

        public TConfig RegisterViewSubmissionHandler(string callbackId, Func<IViewSubmissionHandler> handlerFactory) =>
            RegisterAsyncViewSubmissionHandler(callbackId, () => handlerFactory().ToViewSubmissionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, Func<IAsyncViewSubmissionHandler> handlerFactory) =>
            Chain(() => AddViewSubmissionHandler(callbackId, handlerFactory));

        public TConfig RegisterSlashCommandHandler(string command, Func<ISlashCommandHandler> handlerFactory) =>
            RegisterAsyncSlashCommandHandler(command, () => handlerFactory().ToSlashCommandHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler(string command, Func<IAsyncSlashCommandHandler> handlerFactory)
        {
            ValidateCommandName(command);
            return Chain(() => AddSlashCommandHandler(command, handlerFactory));
        }

        public TConfig RegisterWorkflowStepEditHandler(string callbackId, Func<IWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(callbackId, () => handlerFactory().ToWorkflowStepEditHandler());

        public TConfig RegisterWorkflowStepEditHandler(Func<IWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(() => handlerFactory().ToWorkflowStepEditHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(string callbackId, Func<IAsyncWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(() => handlerFactory().ToWorkflowStepEditHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(Func<IAsyncWorkflowStepEditHandler> handlerFactory) =>
            Chain(() => AddWorkflowStepEditHandler(handlerFactory));

        public TConfig RegisterInteractiveMessageHandler(string actionName, Func<IInteractiveMessageHandler> handlerFactory) =>
            Chain(() => AddLegacyInteractiveMessageHandler(actionName, handlerFactory));

        public TConfig RegisterOptionProvider(string actionName, Func<IOptionProvider> providerFactory) =>
            Chain(() => AddLegacyOptionProvider(actionName, providerFactory));

        public TConfig RegisterDialogSubmissionHandler(string callbackId, Func<IDialogSubmissionHandler> handlerFactory) =>
            Chain(() => AddLegacyDialogSubmissionHandler(callbackId, handlerFactory));

        protected virtual void AddEventHandler(Func<IEventHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddBlockActionHandler(Func<IAsyncBlockActionHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddBlockOptionProvider(string actionId, Func<IBlockOptionProvider> providerFactory) => AddKeyedHandler(actionId, providerFactory);
        protected virtual void AddMessageShortcutHandler(Func<IAsyncMessageShortcutHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddGlobalShortcutHandler(Func<IAsyncGlobalShortcutHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddViewSubmissionHandler(string callbackId, Func<IAsyncViewSubmissionHandler> handlerFactory) => AddKeyedHandler(callbackId, handlerFactory);
        protected virtual void AddSlashCommandHandler(string command, Func<IAsyncSlashCommandHandler> handlerFactory) => AddKeyedHandler(command, handlerFactory);
        protected virtual void AddWorkflowStepEditHandler(Func<IAsyncWorkflowStepEditHandler> handlerFactory) => AddCollectionHandler(handlerFactory);

        protected virtual void AddLegacyInteractiveMessageHandler(string actionName, Func<IInteractiveMessageHandler> handlerFactory) => AddKeyedHandler(actionName, handlerFactory);
        protected virtual void AddLegacyOptionProvider(string actionName, Func<IOptionProvider> providerFactory) => AddKeyedHandler(actionName, providerFactory);
        protected virtual void AddLegacyDialogSubmissionHandler(string callbackId, Func<IDialogSubmissionHandler> handlerFactory) => AddKeyedHandler(callbackId, handlerFactory);

        protected abstract void AddCollectionHandler<THandler>(Func<THandler> handlerFactory) where THandler : class;
        protected abstract void AddKeyedHandler<THandler>(string key, Func<THandler> handlerFactory) where THandler : class;
    }
}