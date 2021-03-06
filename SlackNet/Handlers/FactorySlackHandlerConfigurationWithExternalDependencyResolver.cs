using System;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public abstract class FactorySlackHandlerConfigurationWithExternalDependencyResolver<TConfig> : FactorySlackHandlerConfiguration<TConfig> where TConfig : FactorySlackHandlerConfigurationWithExternalDependencyResolver<TConfig>
    {
        public TConfig UseHttp(Func<IHttp> httpProvider) => UseHttp(GetServiceFactory(httpProvider));
        public TConfig UseJsonSettings(Func<SlackJsonSettings> jsonSettingsProvider) => UseJsonSettings(GetServiceFactory(jsonSettingsProvider));
        public TConfig UseTypeResolver(Func<ISlackTypeResolver> slackTypeResolverProvider) => UseTypeResolver(GetServiceFactory(slackTypeResolverProvider));
        public TConfig UseUrlBuilder(Func<ISlackUrlBuilder> urlBuilderProvider) => UseUrlBuilder(GetServiceFactory(urlBuilderProvider));
        public TConfig UseWebSocketFactory(Func<IWebSocketFactory> webSocketFactoryProvider) => UseWebSocketFactory(GetServiceFactory(webSocketFactoryProvider));
        public TConfig UseRequestContextFactory(Func<ISlackRequestContextFactory> requestContextFactoryProvider) => UseRequestContextFactory(GetServiceFactory(requestContextFactoryProvider));
        public TConfig UseRequestListener(Func<ISlackRequestListener> requestListenerProvider) => UseRequestListener(GetServiceFactory(requestListenerProvider));
        public TConfig UseHandlerFactory(Func<ISlackHandlerFactory> handlerFactoryProvider) => UseHandlerFactory(GetServiceFactory(handlerFactoryProvider));
        public TConfig UseApiClient(Func<ISlackApiClient> apiClientProvider) => UseApiClient(GetServiceFactory(apiClientProvider));
        public TConfig UseSocketModeClient(Func<ISlackSocketModeClient> socketModeClientProvider) => UseSocketModeClient(GetServiceFactory(socketModeClientProvider));

        public TConfig ReplaceEventHandling(Func<IEventHandler> handlerFactory) => ReplaceEventHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceBlockActionHandling(Func<IAsyncBlockActionHandler> handlerFactory) => ReplaceBlockActionHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceBlockOptionProviding(Func<IBlockOptionProvider> providerFactory) => ReplaceBlockOptionProviding(GetRequestHandlerFactory(providerFactory));
        public TConfig ReplaceMessageShortcutHandling(Func<IAsyncMessageShortcutHandler> handlerFactory) => ReplaceMessageShortcutHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceGlobalShortcutHandling(Func<IAsyncGlobalShortcutHandler> handlerFactory) => ReplaceGlobalShortcutHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceViewSubmissionHandling(Func<IAsyncViewSubmissionHandler> handlerFactory) => ReplaceViewSubmissionHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceSlashCommandHandling(Func<IAsyncSlashCommandHandler> handlerFactory) => ReplaceSlashCommandHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceWorkflowStepEditHandling(Func<IAsyncWorkflowStepEditHandler> handlerFactory) => ReplaceWorkflowStepEditHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceLegacyInteractiveMessageHandling(Func<IInteractiveMessageHandler> handlerFactory) => ReplaceLegacyInteractiveMessageHandling(GetRequestHandlerFactory(handlerFactory));
        public TConfig ReplaceLegacyOptionProviding(Func<IOptionProvider> providerFactory) => ReplaceLegacyOptionProviding(GetRequestHandlerFactory(providerFactory));
        public TConfig ReplaceLegacyDialogSubmissionHandling(Func<IDialogSubmissionHandler> handlerFactory) => ReplaceLegacyDialogSubmissionHandling(GetRequestHandlerFactory(handlerFactory));

        public TConfig RegisterEventHandler<TEvent>(Func<IEventHandler<TEvent>> handlerFactory) where TEvent : Event =>
            RegisterEventHandler(() => handlerFactory().ToEventHandler());

        public TConfig RegisterEventHandler(Func<IEventHandler> getHandler) =>
            RegisterEventHandler(GetRequestHandlerFactory(getHandler));

        public TConfig RegisterBlockActionHandler<TAction>(string actionId, Func<IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
            RegisterBlockActionHandler(actionId, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterBlockActionHandler<TAction>(Func<IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
            RegisterBlockActionHandler(GetRequestHandlerFactory(getHandler));

        public TConfig RegisterBlockActionHandler(Func<IBlockActionHandler> getHandler) =>
            RegisterBlockActionHandler(GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(actionId, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler<TAction>(Func<IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
            RegisterAsyncBlockActionHandler(GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncBlockActionHandler(Func<IAsyncBlockActionHandler> getHandler) =>
            RegisterAsyncBlockActionHandler(GetRequestHandlerFactory(getHandler));

        public TConfig RegisterBlockOptionProvider(string actionId, Func<IBlockOptionProvider> getProvider) =>
            RegisterBlockOptionProvider(actionId, GetRequestHandlerFactory(getProvider));

        public TConfig RegisterMessageShortcutHandler(string callbackId, Func<IMessageShortcutHandler> getHandler) =>
            RegisterMessageShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterMessageShortcutHandler(Func<IMessageShortcutHandler> getHandler) =>
            RegisterMessageShortcutHandler(GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, Func<IAsyncMessageShortcutHandler> getHandler) =>
            RegisterAsyncMessageShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncMessageShortcutHandler(Func<IAsyncMessageShortcutHandler> getHandler) =>
            RegisterAsyncMessageShortcutHandler(GetRequestHandlerFactory(getHandler));

        public TConfig RegisterGlobalShortcutHandler(string callbackId, Func<IGlobalShortcutHandler> getHandler) =>
            RegisterGlobalShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterGlobalShortcutHandler(Func<IGlobalShortcutHandler> getHandler) =>
            RegisterGlobalShortcutHandler(GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, Func<IAsyncGlobalShortcutHandler> getHandler) =>
            RegisterAsyncGlobalShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncGlobalShortcutHandler(Func<IAsyncGlobalShortcutHandler> getHandler) =>
            RegisterAsyncGlobalShortcutHandler(GetRequestHandlerFactory(getHandler));

        public TConfig RegisterViewSubmissionHandler(string callbackId, Func<IViewSubmissionHandler> getHandler) =>
            RegisterViewSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, Func<IAsyncViewSubmissionHandler> getHandler) =>
            RegisterAsyncViewSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterSlashCommandHandler(string command, Func<ISlashCommandHandler> getHandler) =>
            RegisterSlashCommandHandler(command, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncSlashCommandHandler(string command, Func<IAsyncSlashCommandHandler> getHandler) =>
            RegisterAsyncSlashCommandHandler(command, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterWorkflowStepEditHandler(string callbackId, Func<IWorkflowStepEditHandler> getHandler) =>
            RegisterWorkflowStepEditHandler(callbackId, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterWorkflowStepEditHandler(Func<IWorkflowStepEditHandler> getHandler) =>
            RegisterWorkflowStepEditHandler(GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(string callbackId, Func<IAsyncWorkflowStepEditHandler> getHandler) =>
            RegisterAsyncWorkflowStepEditHandler(callbackId, GetRequestHandlerFactory(getHandler));

        [Obsolete(Warning.Experimental)]
        public TConfig RegisterAsyncWorkflowStepEditHandler(Func<IAsyncWorkflowStepEditHandler> handlerFactory) =>
            RegisterAsyncWorkflowStepEditHandler(GetRequestHandlerFactory(handlerFactory));

        public TConfig RegisterInteractiveMessageHandler(string actionName, Func<IInteractiveMessageHandler> getHandler) =>
            RegisterInteractiveMessageHandler(actionName, GetRequestHandlerFactory(getHandler));

        public TConfig RegisterOptionProvider(string actionName, Func<IOptionProvider> getProvider) =>
            RegisterOptionProvider(actionName, GetRequestHandlerFactory(getProvider));

        public TConfig RegisterDialogSubmissionHandler(string callbackId, Func<IDialogSubmissionHandler> getHandler) =>
            RegisterDialogSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

        /// <summary>
        /// Get a service factory for the given service callback. The service will only be created once.
        /// </summary>
        protected abstract Func<ISlackServiceFactory, TService> GetServiceFactory<TService>(Func<TService> getService) where TService : class;

        /// <summary>
        /// Get a factory for creating a handler for a request. The handler will be created once per request.
        /// </summary>
        protected abstract Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<THandler> getHandler) where THandler : class;
    }
}