using System;
using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class FactorySlackHandlerConfigurationWithDependencyResolver<TConfig, TDependencyResolver> : FactorySlackHandlerConfiguration<TConfig> where TConfig : FactorySlackHandlerConfigurationWithDependencyResolver<TConfig, TDependencyResolver>
    {
        public TConfig UseHttp(Func<TDependencyResolver, IHttp> httpProvider) => Chain(() => ReplaceClientService(httpProvider));
        public TConfig UseJsonSettings(Func<TDependencyResolver, SlackJsonSettings> jsonSettingsProvider) => Chain(() => ReplaceClientService(jsonSettingsProvider));
        public TConfig UseTypeResolver(Func<TDependencyResolver, ISlackTypeResolver> slackTypeResolverProvider) => Chain(() => ReplaceClientService(slackTypeResolverProvider));
        public TConfig UseUrlBuilder(Func<TDependencyResolver, ISlackUrlBuilder> urlBuilderProvider) => Chain(() => ReplaceClientService(urlBuilderProvider));
        public TConfig UseWebSocketFactory(Func<TDependencyResolver, IWebSocketFactory> webSocketFactoryProvider) => Chain(() => ReplaceClientService(webSocketFactoryProvider));
        public TConfig UseRequestListener(Func<TDependencyResolver, ISlackRequestListener> requestListenerProvider) => Chain(() => ReplaceClientService(requestListenerProvider));
        public TConfig UseHandlerFactory(Func<TDependencyResolver, ISlackHandlerFactory> handlerFactoryProvider) => Chain(() => ReplaceClientService(handlerFactoryProvider));
        public TConfig UseApiClient(Func<TDependencyResolver, ISlackApiClient> apiClientProvider) => Chain(() => ReplaceClientService(apiClientProvider));
        public TConfig UseSocketModeClient(Func<TDependencyResolver, ISlackSocketModeClient> socketModeClientProvider) => Chain(() => ReplaceClientService(socketModeClientProvider));

        public TConfig ReplaceEventHandling(CollectionHandlerFactory<TDependencyResolver, IEventHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public TConfig ReplaceBlockActionHandling(CollectionHandlerFactory<TDependencyResolver, IAsyncBlockActionHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public TConfig ReplaceBlockOptionProviding(KeyedHandlerFactory<TDependencyResolver, IBlockOptionProvider> providerFactory) => Chain(() => ReplaceKeyedHandling(providerFactory));
        public TConfig ReplaceMessageShortcutHandling(CollectionHandlerFactory<TDependencyResolver, IAsyncMessageShortcutHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public TConfig ReplaceGlobalShortcutHandling(CollectionHandlerFactory<TDependencyResolver, IAsyncGlobalShortcutHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public TConfig ReplaceViewSubmissionHandling(KeyedHandlerFactory<TDependencyResolver, IAsyncViewSubmissionHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public TConfig ReplaceSlashCommandHandling(KeyedHandlerFactory<TDependencyResolver, IAsyncSlashCommandHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public TConfig ReplaceWorkflowStepEditHandling(CollectionHandlerFactory<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public TConfig ReplaceLegacyInteractiveMessageHandling(KeyedHandlerFactory<TDependencyResolver, IInteractiveMessageHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public TConfig ReplaceLegacyOptionProviding(KeyedHandlerFactory<TDependencyResolver, IOptionProvider> providerFactory) => Chain(() => ReplaceKeyedHandling(providerFactory));
        public TConfig ReplaceLegacyDialogSubmissionHandling(KeyedHandlerFactory<TDependencyResolver, IDialogSubmissionHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));

        public TConfig RegisterEventHandler<TEvent>(Func<TDependencyResolver, IEventHandler<TEvent>> handlerFactory) where TEvent : Event =>
            RegisterEventHandler(r => handlerFactory(r).ToEventHandler());

        public TConfig RegisterEventHandler(Func<TDependencyResolver, IEventHandler> handlerFactory) =>
            Chain(() => AddEventHandler(handlerFactory));

        public TConfig RegisterBlockActionHandler<TAction>(string actionId, Func<TDependencyResolver, IBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(r => handlerFactory(r).ToBlockActionHandler(actionId));

        public TConfig RegisterBlockActionHandler<TAction>(Func<TDependencyResolver, IBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(r => handlerFactory(r).ToBlockActionHandler());

        public TConfig RegisterBlockActionHandler(Func<TDependencyResolver, IBlockActionHandler> handlerFactory) =>
            RegisterAsyncBlockActionHandler(r => handlerFactory(r).ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<TDependencyResolver, IAsyncBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(r => handlerFactory(r).ToBlockActionHandler(actionId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(Func<TDependencyResolver, IAsyncBlockActionHandler<TAction>> handlerFactory) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(r => handlerFactory(r).ToBlockActionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler(Func<TDependencyResolver, IAsyncBlockActionHandler> handlerFactory) =>
            Chain(() => AddBlockActionHandler(handlerFactory));

        public TConfig RegisterBlockOptionProvider(string actionId, Func<TDependencyResolver, IBlockOptionProvider> providerFactory) =>
            Chain(() => AddBlockOptionProvider(actionId, providerFactory));

        public TConfig RegisterMessageShortcutHandler(string callbackId, Func<TDependencyResolver, IMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(callbackId, r => handlerFactory(r).ToMessageShortcutHandler());

        public TConfig RegisterMessageShortcutHandler(Func<TDependencyResolver, IMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(r => handlerFactory(r).ToMessageShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, Func<TDependencyResolver, IAsyncMessageShortcutHandler> handlerFactory) =>
            RegisterAsyncMessageShortcutHandler(r => handlerFactory(r).ToMessageShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(Func<TDependencyResolver, IAsyncMessageShortcutHandler> handlerFactory) =>
            Chain(() => AddMessageShortcutHandler(handlerFactory));

        public TConfig RegisterGlobalShortcutHandler(string callbackId, Func<TDependencyResolver, IGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(callbackId, r => handlerFactory(r).ToGlobalShortcutHandler());

        public TConfig RegisterGlobalShortcutHandler(Func<TDependencyResolver, IGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(r => handlerFactory(r).ToGlobalShortcutHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, Func<TDependencyResolver, IAsyncGlobalShortcutHandler> handlerFactory) =>
            RegisterAsyncGlobalShortcutHandler(r => handlerFactory(r).ToGlobalShortcutHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(Func<TDependencyResolver, IAsyncGlobalShortcutHandler> handlerFactory) =>
            Chain(() => AddGlobalShortcutHandler(handlerFactory));

        public TConfig RegisterViewSubmissionHandler(string callbackId, Func<TDependencyResolver, IViewSubmissionHandler> handlerFactory) =>
            RegisterAsyncViewSubmissionHandler(callbackId, r => handlerFactory(r).ToViewSubmissionHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, Func<TDependencyResolver, IAsyncViewSubmissionHandler> handlerFactory) =>
            Chain(() => AddViewSubmissionHandler(callbackId, handlerFactory));

        public TConfig RegisterSlashCommandHandler(string command, Func<TDependencyResolver, ISlashCommandHandler> handlerFactory) =>
            RegisterAsyncSlashCommandHandler(command, r => handlerFactory(r).ToSlashCommandHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler(string command, Func<TDependencyResolver, IAsyncSlashCommandHandler> handlerFactory)
        {
            ValidateCommandName(command);
            return Chain(() => AddSlashCommandHandler(command, handlerFactory));
        }

        public TConfig RegisterWorkflowStepEditHandler(string callbackId, Func<TDependencyResolver, IWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(callbackId, r => handlerFactory(r).ToWorkflowStepEditHandler());

        public TConfig RegisterWorkflowStepEditHandler(Func<TDependencyResolver, IWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(r => handlerFactory(r).ToWorkflowStepEditHandler());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(string callbackId, Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(r => handlerFactory(r).ToWorkflowStepEditHandler(callbackId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) =>
            Chain(() => AddWorkflowStepEditHandler(handlerFactory));

        public TConfig RegisterInteractiveMessageHandler(string actionName, Func<TDependencyResolver, IInteractiveMessageHandler> handlerFactory) =>
            Chain(() => AddLegacyInteractiveMessageHandler(actionName, handlerFactory));

        public TConfig RegisterOptionProvider(string actionName, Func<TDependencyResolver, IOptionProvider> providerFactory) =>
            Chain(() => AddLegacyOptionProvider(actionName, providerFactory));

        public TConfig RegisterDialogSubmissionHandler(string callbackId, Func<TDependencyResolver, IDialogSubmissionHandler> handlerFactory) =>
            Chain(() => AddLegacyDialogSubmissionHandler(callbackId, handlerFactory));

        protected override void ReplaceClientService<TService>(Func<TService> serviceFactory) => ReplaceClientService(r => serviceFactory());
        protected override void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<THandler> handlerFactory) => ReplaceCollectionHandling<THandler>((r, hs) => handlerFactory(hs));
        protected override void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<THandler> handlerFactory) => ReplaceKeyedHandling<THandler>((r, hs) => handlerFactory(hs));

        protected virtual void AddEventHandler(Func<TDependencyResolver, IEventHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddBlockActionHandler(Func<TDependencyResolver, IAsyncBlockActionHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddBlockOptionProvider(string actionId, Func<TDependencyResolver, IBlockOptionProvider> providerFactory) => AddKeyedHandler(actionId, providerFactory);
        protected virtual void AddMessageShortcutHandler(Func<TDependencyResolver, IAsyncMessageShortcutHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddGlobalShortcutHandler(Func<TDependencyResolver, IAsyncGlobalShortcutHandler> handlerFactory) => AddCollectionHandler(handlerFactory);
        protected virtual void AddViewSubmissionHandler(string callbackId, Func<TDependencyResolver, IAsyncViewSubmissionHandler> handlerFactory) => AddKeyedHandler(callbackId, handlerFactory);
        protected virtual void AddSlashCommandHandler(string command, Func<TDependencyResolver, IAsyncSlashCommandHandler> handlerFactory) => AddKeyedHandler(command, handlerFactory);
        protected virtual void AddWorkflowStepEditHandler(Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) => AddCollectionHandler(handlerFactory);

        protected virtual void AddLegacyInteractiveMessageHandler(string actionName, Func<TDependencyResolver, IInteractiveMessageHandler> handlerFactory) => AddKeyedHandler(actionName, handlerFactory);
        protected virtual void AddLegacyOptionProvider(string actionName, Func<TDependencyResolver, IOptionProvider> providerFactory) => AddKeyedHandler(actionName, providerFactory);
        protected virtual void AddLegacyDialogSubmissionHandler(string callbackId, Func<TDependencyResolver, IDialogSubmissionHandler> handlerFactory) => AddKeyedHandler(callbackId, handlerFactory);

        protected abstract void ReplaceClientService<TService>(Func<TDependencyResolver, TService> serviceFactory) where TService : class;
        protected abstract void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<TDependencyResolver, THandler> handlerFactory) where THandler : class;
        protected abstract void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<TDependencyResolver, THandler> handlerFactory) where THandler : class;
        protected abstract void AddCollectionHandler<THandler>(Func<TDependencyResolver, THandler> handlerFactory) where THandler : class;
        protected abstract void AddKeyedHandler<THandler>(string key, Func<TDependencyResolver, THandler> handlerFactory) where THandler : class;
    }

    public delegate THandler CollectionHandlerFactory<in TDependencyResolver, THandler>(TDependencyResolver dependencyResolver, IEnumerable<THandler> registeredHandlers);
    public delegate THandler KeyedHandlerFactory<in TDependencyResolver, THandler>(TDependencyResolver dependencyResolver, IHandlerIndex<THandler> registeredHandlers);
}