using System;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class FactorySlackHandlerConfiguration<TConfig> : SlackHandlerConfigurationBase<TConfig> where TConfig : FactorySlackHandlerConfiguration<TConfig>
    {
        public override TConfig UseHttp(Func<IHttp> httpProvider) => Chain(() => ReplaceClientService(httpProvider));
        public override TConfig UseJsonSettings(Func<SlackJsonSettings> jsonSettingsProvider) => Chain(() => ReplaceClientService(jsonSettingsProvider));
        public override TConfig UseTypeResolver(Func<ISlackTypeResolver> slackTypeResolverProvider) => Chain(() => ReplaceClientService(slackTypeResolverProvider));
        public override TConfig UseUrlBuilder(Func<ISlackUrlBuilder> urlBuilderProvider) => Chain(() => ReplaceClientService(urlBuilderProvider));
        public override TConfig UseWebSocketFactory(Func<IWebSocketFactory> webSocketFactoryProvider) => Chain(() => ReplaceClientService(webSocketFactoryProvider));
        public override TConfig UseRequestListener(Func<ISlackRequestListener> requestListenerProvider) => Chain(() => ReplaceClientService(requestListenerProvider));
        public override TConfig UseHandlerFactory(Func<ISlackHandlerFactory> handlerFactoryProvider) => Chain(() => ReplaceClientService(handlerFactoryProvider));
        public override TConfig UseApiClient(Func<ISlackApiClient> apiClientProvider) => Chain(() => ReplaceClientService(apiClientProvider));
        public override TConfig UseSocketModeClient(Func<ISlackSocketModeClient> socketModeClientProvider) => Chain(() => ReplaceClientService(socketModeClientProvider));

        public virtual TConfig UseHttp<TService>() where TService : class, IHttp => Chain(ReplaceClientService<IHttp, TService>);
        public virtual TConfig UseJsonSettings<TService>() where TService : SlackJsonSettings => Chain(ReplaceClientService<SlackJsonSettings, TService>);
        public virtual TConfig UseTypeResolver<TService>() where TService : class, ISlackTypeResolver => Chain(ReplaceClientService<ISlackTypeResolver, TService>);
        public virtual TConfig UseUrlBuilder<TService>() where TService : class, ISlackUrlBuilder => Chain(ReplaceClientService<ISlackUrlBuilder, TService>);
        public virtual TConfig UseWebSocketFactory<TService>() where TService : class, IWebSocketFactory => Chain(ReplaceClientService<IWebSocketFactory, TService>);
        public virtual TConfig UseRequestListener<TService>() where TService : class, ISlackRequestListener => Chain(ReplaceClientService<ISlackRequestListener, TService>);
        public virtual TConfig UseHandlerFactory<TService>() where TService : class, ISlackHandlerFactory => Chain(ReplaceClientService<ISlackHandlerFactory, TService>);
        public virtual TConfig UseApiClient<TService>() where TService : class, ISlackApiClient => Chain(ReplaceClientService<ISlackApiClient, TService>);
        public virtual TConfig UseSocketModeClient<TService>() where TService : class, ISlackSocketModeClient => Chain(ReplaceClientService<ISlackSocketModeClient, TService>);

        public override TConfig ReplaceEventHandling(CollectionHandlerFactory<IEventHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public override TConfig ReplaceBlockActionHandling(CollectionHandlerFactory<IAsyncBlockActionHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public override TConfig ReplaceBlockOptionProviding(KeyedHandlerFactory<IBlockOptionProvider> providerFactory) => Chain(() => ReplaceKeyedHandling(providerFactory));
        public override TConfig ReplaceMessageShortcutHandling(CollectionHandlerFactory<IAsyncMessageShortcutHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public override TConfig ReplaceGlobalShortcutHandling(CollectionHandlerFactory<IAsyncGlobalShortcutHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public override TConfig ReplaceViewSubmissionHandling(KeyedHandlerFactory<IAsyncViewSubmissionHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public override TConfig ReplaceSlashCommandHandling(KeyedHandlerFactory<IAsyncSlashCommandHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public override TConfig ReplaceWorkflowStepEditHandling(CollectionHandlerFactory<IAsyncWorkflowStepEditHandler> handlerFactory) => Chain(() => ReplaceCollectionHandling(handlerFactory));
        public override TConfig ReplaceLegacyInteractiveMessageHandling(KeyedHandlerFactory<IInteractiveMessageHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));
        public override TConfig ReplaceLegacyOptionProviding(KeyedHandlerFactory<IOptionProvider> providerFactory) => Chain(() => ReplaceKeyedHandling(providerFactory));
        public override TConfig ReplaceLegacyDialogSubmissionHandling(KeyedHandlerFactory<IDialogSubmissionHandler> handlerFactory) => Chain(() => ReplaceKeyedHandling(handlerFactory));

        /// <summary>
        /// Take over all event handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceEventHandling<THandler>() where THandler : class, IEventHandler =>
            Chain(ReplaceCollectionHandling<IEventHandler, THandler>);

        /// <summary>
        /// Take over all block action handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceBlockActionHandling<THandler>() where THandler : class, IAsyncBlockActionHandler =>
            Chain(ReplaceCollectionHandling<IAsyncBlockActionHandler, THandler>);

        /// <summary>
        /// Take over all block option providing with your own provider type.
        /// </summary>
        public virtual TConfig ReplaceBlockOptionProviding<TProvider>() where TProvider : class, IBlockOptionProvider =>
            Chain(ReplaceKeyedHandler<IBlockOptionProvider, TProvider>);

        /// <summary>
        /// Take over all message shortcut handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceMessageShortcutHandling<THandler>() where THandler : class, IAsyncMessageShortcutHandler =>
            Chain(ReplaceCollectionHandling<IAsyncMessageShortcutHandler, THandler>);

        /// <summary>
        /// Take over all global shortcut handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceGlobalShortcutHandling<THandler>() where THandler : class, IAsyncGlobalShortcutHandler =>
            Chain(ReplaceCollectionHandling<IAsyncGlobalShortcutHandler, THandler>);

        /// <summary>
        /// Take over all view submission handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceViewSubmissionHandling<THandler>() where THandler : class, IAsyncViewSubmissionHandler =>
            Chain(ReplaceKeyedHandler<IAsyncViewSubmissionHandler, THandler>);

        /// <summary>
        /// Take over all slash command handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceSlashCommandHandling<THandler>() where THandler : class, IAsyncSlashCommandHandler =>
            Chain(ReplaceKeyedHandler<IAsyncSlashCommandHandler, THandler>);

        /// <summary>
        /// Take over all workflow Step Edit handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceWorkflowStepEditHandling<THandler>() where THandler : class, IAsyncWorkflowStepEditHandler =>
            Chain(ReplaceCollectionHandling<IAsyncWorkflowStepEditHandler, THandler>);

        /// <summary>
        /// Take over all interactive message handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceLegacyInteractiveMessageHandling<THandler>() where THandler : class, IInteractiveMessageHandler =>
            Chain(ReplaceKeyedHandler<IInteractiveMessageHandler, THandler>);

        /// <summary>
        /// Take over all legacy option providing with your own provider type.
        /// </summary>
        public virtual TConfig ReplaceLegacyOptionProviding<TProvider>() where TProvider : class, IOptionProvider =>
            Chain(ReplaceKeyedHandler<IOptionProvider, TProvider>);

        /// <summary>
        /// Take over all legacy dialog submission handling with your own handler type.
        /// </summary>
        public virtual TConfig ReplaceLegacyDialogSubmissionHandling<THandler>() where THandler : class, IDialogSubmissionHandler =>
            Chain(ReplaceKeyedHandler<IDialogSubmissionHandler, THandler>);

        public TConfig RegisterEventHandler<THandler>()
            where THandler : class, IEventHandler =>
            Chain(() => AddEventHandler<THandler>(h => h));

        public TConfig RegisterEventHandler<TEvent, THandler>()
            where TEvent : Event
            where THandler : class, IEventHandler<TEvent> =>
            Chain(() => AddEventHandler<THandler>(h => h.ToEventHandler()));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<THandler>()
            where THandler : class, IAsyncBlockActionHandler =>
            Chain(() => AddBlockActionHandler<THandler>(h => h));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction> =>
            Chain(() => AddBlockActionHandler<THandler>(h => h.ToBlockActionHandler()));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction> =>
            Chain(() => AddBlockActionHandler<THandler>(h => h.ToBlockActionHandler(actionId)));

        public TConfig RegisterBlockActionHandler<THandler>()
            where THandler : class, IBlockActionHandler =>
            Chain(() => AddBlockActionHandler<THandler>(h => h.ToBlockActionHandler()));

        public TConfig RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction> =>
            Chain(() => AddBlockActionHandler<THandler>(h => h.ToBlockActionHandler()));

        public TConfig RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction> =>
            Chain(() => AddBlockActionHandler<THandler>(h => h.ToBlockActionHandler(actionId)));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler<THandler>()
            where THandler : class, IAsyncMessageShortcutHandler =>
            Chain(() => AddMessageShortcutHandler<THandler>(h => h));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncMessageShortcutHandler =>
            Chain(() => AddMessageShortcutHandler<THandler>(h => h.ToMessageShortcutHandler(callbackId)));

        public TConfig RegisterMessageShortcutHandler<THandler>()
            where THandler : class, IMessageShortcutHandler =>
            Chain(() => AddMessageShortcutHandler<THandler>(h => h.ToMessageShortcutHandler()));

        public TConfig RegisterMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IMessageShortcutHandler =>
            Chain(() => AddMessageShortcutHandler<THandler>(h => h.ToMessageShortcutHandler(callbackId)));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler<THandler>()
            where THandler : class, IAsyncGlobalShortcutHandler =>
            Chain(() => AddGlobalShortcutHandler<THandler>(h => h));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncGlobalShortcutHandler =>
            Chain(() => AddGlobalShortcutHandler<THandler>(h => h.ToGlobalShortcutHandler(callbackId)));

        public TConfig RegisterGlobalShortcutHandler<THandler>()
            where THandler : class, IGlobalShortcutHandler =>
            Chain(() => AddGlobalShortcutHandler<THandler>(h => h.ToGlobalShortcutHandler()));

        public TConfig RegisterGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IGlobalShortcutHandler =>
            Chain(() => AddGlobalShortcutHandler<THandler>(h => h.ToGlobalShortcutHandler(callbackId)));

        public TConfig RegisterBlockOptionProvider<TProvider>(string actionId)
            where TProvider : class, IBlockOptionProvider =>
            Chain(() => AddBlockOptionProvider<TProvider>(actionId));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IAsyncViewSubmissionHandler =>
            Chain(() => AddViewSubmissionHandler<THandler>(callbackId, h => h));

        public TConfig RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler =>
            Chain(() => AddViewSubmissionHandler<THandler>(callbackId, h => h.ToViewSubmissionHandler()));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler<THandler>(string command)
            where THandler : class, IAsyncSlashCommandHandler
        {
            ValidateCommandName(command);
            return Chain(() => AddSlashCommandHandler<THandler>(command, h => h));
        }

        public TConfig RegisterSlashCommandHandler<THandler>(string command)
            where THandler : class, ISlashCommandHandler
        {
            ValidateCommandName(command);
            return Chain(() => AddSlashCommandHandler<THandler>(command, h => h.ToSlashCommandHandler()));
        }

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler<THandler>()
            where THandler : class, IAsyncWorkflowStepEditHandler =>
            Chain(() => AddWorkflowStepEditHandler<THandler>(h => h));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler<THandler>(string callbackId)
            where THandler : class, IAsyncWorkflowStepEditHandler =>
            Chain(() => AddWorkflowStepEditHandler<THandler>(h => h.ToWorkflowStepEditHandler(callbackId)));

        public TConfig RegisterWorkflowStepEditHandler<THandler>()
            where THandler : class, IWorkflowStepEditHandler =>
            Chain(() => AddWorkflowStepEditHandler<THandler>(h => h.ToWorkflowStepEditHandler()));

        public TConfig RegisterWorkflowStepEditHandler<THandler>(string callbackId)
            where THandler : class, IWorkflowStepEditHandler =>
            Chain(() => AddWorkflowStepEditHandler<THandler>(h => h.ToWorkflowStepEditHandler(callbackId)));

        public TConfig RegisterInteractiveMessageHandler<THandler>(string actionName)
            where THandler : class, IInteractiveMessageHandler =>
            Chain(() => AddLegacyInteractiveMessageHandler<THandler>(actionName));

        public TConfig RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider =>
            Chain(() => AddLegacyOptionProvider<TProvider>(actionName));

        public TConfig RegisterDialogSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IDialogSubmissionHandler =>
            Chain(() => AddLegacyDialogSubmissionHandler<THandler>(callbackId));

        protected override void AddEventHandler(IEventHandler handler) => AddCollectionHandler(handler);
        protected override void AddBlockActionHandler(IAsyncBlockActionHandler handler) => AddCollectionHandler(handler);
        protected override void AddBlockOptionProvider(string actionId, IBlockOptionProvider provider) => AddKeyedHandler(actionId, provider);
        protected override void AddMessageShortcutHandler(IAsyncMessageShortcutHandler handler) => AddCollectionHandler(handler);
        protected override void AddGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler) => AddCollectionHandler(handler);
        protected override void AddViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler) => AddKeyedHandler(callbackId, handler);
        protected override void AddSlashCommandHandler(string command, IAsyncSlashCommandHandler handler) => AddKeyedHandler(command, handler);
        protected override void AddWorkflowStepEditHandler(IAsyncWorkflowStepEditHandler handler) => AddCollectionHandler(handler);

        protected override void AddLegacyInteractiveMessageHandler(string actionName, IInteractiveMessageHandler handler) => AddKeyedHandler(actionName, handler);
        protected override void AddLegacyOptionProvider(string actionName, IOptionProvider provider) => AddKeyedHandler(actionName, provider);
        protected override void AddLegacyDialogSubmissionHandler(string callbackId, IDialogSubmissionHandler handler) => AddKeyedHandler(callbackId, handler);

        protected virtual void AddEventHandler<THandler>(Func<THandler, IEventHandler> adaptor) where THandler : class => AddCollectionHandler(adaptor);
        protected virtual void AddBlockActionHandler<THandler>(Func<THandler, IAsyncBlockActionHandler> adaptor) where THandler : class => AddCollectionHandler(adaptor);
        protected virtual void AddBlockOptionProvider<THandler>(string actionId) where THandler: class, IBlockOptionProvider => AddKeyedHandler<THandler, IBlockOptionProvider>(actionId, h => h);
        protected virtual void AddMessageShortcutHandler<THandler>(Func<THandler, IAsyncMessageShortcutHandler> adaptor) where THandler : class => AddCollectionHandler(adaptor);
        protected virtual void AddGlobalShortcutHandler<THandler>(Func<THandler, IAsyncGlobalShortcutHandler> adaptor) where THandler : class => AddCollectionHandler(adaptor);
        protected virtual void AddViewSubmissionHandler<THandler>(string callbackId, Func<THandler, IAsyncViewSubmissionHandler> adaptor) where THandler : class => AddKeyedHandler(callbackId, adaptor);
        protected virtual void AddSlashCommandHandler<THandler>(string command, Func<THandler, IAsyncSlashCommandHandler> adaptor) where THandler : class => AddKeyedHandler(command, adaptor);
        protected virtual void AddWorkflowStepEditHandler<THandler>(Func<THandler, IAsyncWorkflowStepEditHandler> adaptor) where THandler : class => AddCollectionHandler(adaptor);

        protected virtual void AddLegacyInteractiveMessageHandler<THandler>(string actionName) where THandler : class, IInteractiveMessageHandler => AddKeyedHandler<THandler, IInteractiveMessageHandler>(actionName, h => h);
        protected virtual void AddLegacyOptionProvider<THandler>(string actionName) where THandler : class, IOptionProvider => AddKeyedHandler<THandler, IOptionProvider>(actionName, h => h);
        protected virtual void AddLegacyDialogSubmissionHandler<THandler>(string callbackId) where THandler : class, IDialogSubmissionHandler => AddKeyedHandler<THandler, IDialogSubmissionHandler>(callbackId, h => h);

        protected abstract void ReplaceClientService<TService>(Func<TService> serviceFactory) where TService : class;
        protected abstract void ReplaceClientService<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        protected abstract void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<THandler> handlerFactory) where THandler : class;
        protected abstract void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<THandler> handlerFactory) where THandler : class;
        protected abstract void ReplaceCollectionHandling<THandler, TImplementation>() where THandler : class where TImplementation : class, THandler;
        protected abstract void ReplaceKeyedHandler<THandler, TImplementation>() where THandler : class where TImplementation : class, THandler;
        protected abstract void AddCollectionHandler<THandler>(THandler handler) where THandler : class;
        protected abstract void AddKeyedHandler<THandler>(string key, THandler handler) where THandler : class;
        protected abstract void AddCollectionHandler<TInnerHandler, TOuterHandler>(Func<TInnerHandler, TOuterHandler> adaptor) where TInnerHandler : class where TOuterHandler : class;
        protected abstract void AddKeyedHandler<TInnerHandler, TOuterHandler>(string key, Func<TInnerHandler, TOuterHandler> adaptor) where TInnerHandler : class where TOuterHandler : class;
    }
}