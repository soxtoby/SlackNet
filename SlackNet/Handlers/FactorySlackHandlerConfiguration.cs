using System;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class FactorySlackHandlerConfiguration<TConfig> : SlackServiceConfigurationBase<TConfig> where TConfig : FactorySlackHandlerConfiguration<TConfig>
    {
        public TConfig UseHttp<TService>() where TService : class, IHttp => UseHttp(GetServiceFactory<IHttp, TService>());
        public TConfig UseJsonSettings<TService>() where TService : SlackJsonSettings => UseJsonSettings(GetServiceFactory<SlackJsonSettings, TService>());
        public TConfig UseTypeResolver<TService>() where TService : class, ISlackTypeResolver => UseTypeResolver(GetServiceFactory<ISlackTypeResolver, TService>());
        public TConfig UseUrlBuilder<TService>() where TService : class, ISlackUrlBuilder => UseUrlBuilder(GetServiceFactory<ISlackUrlBuilder, TService>());
        public TConfig UseWebSocketFactory<TService>() where TService : class, IWebSocketFactory => UseWebSocketFactory(GetServiceFactory<IWebSocketFactory, TService>());
        public TConfig UseRequestContextFactory<TService>() where TService : class, ISlackRequestContextFactory => UseRequestContextFactory(GetServiceFactory<ISlackRequestContextFactory, TService>());
        public TConfig UseRequestListener<TService>() where TService : class, ISlackRequestListener => UseRequestListener(GetServiceFactory<ISlackRequestListener, TService>());
        public TConfig UseHandlerFactory<TService>() where TService : class, ISlackHandlerFactory => UseHandlerFactory(GetServiceFactory<ISlackHandlerFactory, TService>());
        public TConfig UseApiClient<TService>() where TService : class, ISlackApiClient => UseApiClient(GetServiceFactory<ISlackApiClient, TService>());
        public TConfig UseSocketModeClient<TService>() where TService : class, ISlackSocketModeClient => UseSocketModeClient(GetServiceFactory<ISlackSocketModeClient, TService>());

        /// <summary>
        /// Take over all event handling with your own handler type.
        /// </summary>
        public TConfig ReplaceEventHandling<THandler>() where THandler : class, IEventHandler =>
            ReplaceEventHandling(GetRequestHandlerFactory<IEventHandler, THandler>());

        /// <summary>
        /// Take over all block action handling with your own handler type.
        /// </summary>
        public TConfig ReplaceBlockActionHandling<THandler>() where THandler : class, IAsyncBlockActionHandler =>
            ReplaceBlockActionHandling(GetRequestHandlerFactory<IAsyncBlockActionHandler, THandler>());

        /// <summary>
        /// Take over all block option providing with your own provider type.
        /// </summary>
        public TConfig ReplaceBlockOptionProviding<TProvider>() where TProvider : class, IBlockOptionProvider =>
            ReplaceBlockOptionProviding(GetRequestHandlerFactory<IBlockOptionProvider, TProvider>());

        /// <summary>
        /// Take over all message shortcut handling with your own handler type.
        /// </summary>
        public TConfig ReplaceMessageShortcutHandling<THandler>() where THandler : class, IAsyncMessageShortcutHandler =>
            ReplaceMessageShortcutHandling(GetRequestHandlerFactory<IAsyncMessageShortcutHandler, THandler>());

        /// <summary>
        /// Take over all global shortcut handling with your own handler type.
        /// </summary>
        public TConfig ReplaceGlobalShortcutHandling<THandler>() where THandler : class, IAsyncGlobalShortcutHandler =>
            ReplaceGlobalShortcutHandling(GetRequestHandlerFactory<IAsyncGlobalShortcutHandler, THandler>());

        /// <summary>
        /// Take over all view submission handling with your own handler type.
        /// </summary>
        public TConfig ReplaceViewSubmissionHandling<THandler>() where THandler : class, IAsyncViewSubmissionHandler =>
            ReplaceViewSubmissionHandling(GetRequestHandlerFactory<IAsyncViewSubmissionHandler, THandler>());

        /// <summary>
        /// Take over all slash command handling with your own handler type.
        /// </summary>
        public TConfig ReplaceSlashCommandHandling<THandler>() where THandler : class, IAsyncSlashCommandHandler =>
            ReplaceSlashCommandHandling(GetRequestHandlerFactory<IAsyncSlashCommandHandler, THandler>());

        /// <summary>
        /// Take over all workflow Step Edit handling with your own handler type.
        /// </summary>
        public TConfig ReplaceWorkflowStepEditHandling<THandler>() where THandler : class, IAsyncWorkflowStepEditHandler =>
            ReplaceWorkflowStepEditHandling(GetRequestHandlerFactory<IAsyncWorkflowStepEditHandler, THandler>());

        /// <summary>
        /// Take over all interactive message handling with your own handler type.
        /// </summary>
        public TConfig ReplaceLegacyInteractiveMessageHandling<THandler>() where THandler : class, IInteractiveMessageHandler =>
            ReplaceLegacyInteractiveMessageHandling(GetRequestHandlerFactory<IInteractiveMessageHandler, THandler>());

        /// <summary>
        /// Take over all legacy option providing with your own provider type.
        /// </summary>
        public TConfig ReplaceLegacyOptionProviding<TProvider>() where TProvider : class, IOptionProvider =>
            ReplaceLegacyOptionProviding(GetRequestHandlerFactory<IOptionProvider, TProvider>());

        /// <summary>
        /// Take over all legacy dialog submission handling with your own handler type.
        /// </summary>
        public TConfig ReplaceLegacyDialogSubmissionHandling<THandler>() where THandler : class, IDialogSubmissionHandler =>
            ReplaceLegacyDialogSubmissionHandling(GetRequestHandlerFactory<IDialogSubmissionHandler, THandler>());

        public TConfig RegisterEventHandler<THandler>()
            where THandler : class, IEventHandler =>
            RegisterEventHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterEventHandler<TEvent, THandler>()
            where TEvent : Event
            where THandler : class, IEventHandler<TEvent> =>
            RegisterEventHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<THandler>()
            where THandler : class, IAsyncBlockActionHandler =>
            RegisterAsyncBlockActionHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction> =>
            RegisterAsyncBlockActionHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IAsyncBlockActionHandler<TAction> =>
            RegisterAsyncBlockActionHandler(actionId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterBlockActionHandler<THandler>()
            where THandler : class, IBlockActionHandler =>
            RegisterBlockActionHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterBlockActionHandler<TAction, THandler>()
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction> =>
            RegisterBlockActionHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterBlockActionHandler<TAction, THandler>(string actionId)
            where TAction : BlockAction
            where THandler : class, IBlockActionHandler<TAction> =>
            RegisterBlockActionHandler(actionId, GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler<THandler>()
            where THandler : class, IAsyncMessageShortcutHandler =>
            RegisterAsyncMessageShortcutHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncMessageShortcutHandler =>
            RegisterAsyncMessageShortcutHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterMessageShortcutHandler<THandler>()
            where THandler : class, IMessageShortcutHandler =>
            RegisterMessageShortcutHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterMessageShortcutHandler<THandler>(string callbackId)
            where THandler : class, IMessageShortcutHandler =>
            RegisterMessageShortcutHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler<THandler>()
            where THandler : class, IAsyncGlobalShortcutHandler =>
            RegisterAsyncGlobalShortcutHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IAsyncGlobalShortcutHandler =>
            RegisterAsyncGlobalShortcutHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterGlobalShortcutHandler<THandler>()
            where THandler : class, IGlobalShortcutHandler =>
            RegisterGlobalShortcutHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterGlobalShortcutHandler<THandler>(string callbackId)
            where THandler : class, IGlobalShortcutHandler =>
            RegisterGlobalShortcutHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterBlockOptionProvider<TProvider>(string actionId)
            where TProvider : class, IBlockOptionProvider =>
            RegisterBlockOptionProvider(actionId, GetRegisteredHandlerFactory<TProvider>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IAsyncViewSubmissionHandler =>
            RegisterAsyncViewSubmissionHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterViewSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IViewSubmissionHandler =>
            RegisterViewSubmissionHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler<THandler>(string command)
            where THandler : class, IAsyncSlashCommandHandler =>
            RegisterAsyncSlashCommandHandler(command, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterSlashCommandHandler<THandler>(string command)
            where THandler : class, ISlashCommandHandler =>
            RegisterSlashCommandHandler(command, GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler<THandler>()
            where THandler : class, IAsyncWorkflowStepEditHandler =>
            RegisterAsyncWorkflowStepEditHandler(GetRegisteredHandlerFactory<THandler>());

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler<THandler>(string callbackId)
            where THandler : class, IAsyncWorkflowStepEditHandler =>
            RegisterAsyncWorkflowStepEditHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterWorkflowStepEditHandler<THandler>()
            where THandler : class, IWorkflowStepEditHandler =>
            RegisterWorkflowStepEditHandler(GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterWorkflowStepEditHandler<THandler>(string callbackId)
            where THandler : class, IWorkflowStepEditHandler =>
            RegisterWorkflowStepEditHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterInteractiveMessageHandler<THandler>(string actionName)
            where THandler : class, IInteractiveMessageHandler =>
            RegisterInteractiveMessageHandler(actionName, GetRegisteredHandlerFactory<THandler>());

        public TConfig RegisterOptionProvider<TProvider>(string actionName)
            where TProvider : class, IOptionProvider =>
            RegisterOptionProvider(actionName, GetRegisteredHandlerFactory<TProvider>());

        public TConfig RegisterDialogSubmissionHandler<THandler>(string callbackId)
            where THandler : class, IDialogSubmissionHandler =>
            RegisterDialogSubmissionHandler(callbackId, GetRegisteredHandlerFactory<THandler>());

        /// <summary>
        /// Get a service factory for the given service type. Should provide the service as a singleton.
        /// </summary>
        protected abstract Func<ISlackServiceFactory, TService> GetServiceFactory<TService, TImplementation>() where TService : class where TImplementation : class, TService;

        /// <summary>
        /// Get a factory for creating a top-level handler for a request. Should scope the handler to the request.
        /// </summary>
        protected abstract Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>() where THandler : class where TImplementation : class, THandler;

        /// <summary>
        /// Get a factory for creating an individual registered handler, to be provided to the top-level handler for the request. Should scope the handler to the request.
        /// </summary>
        protected abstract Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>() where THandler : class;
    }
}