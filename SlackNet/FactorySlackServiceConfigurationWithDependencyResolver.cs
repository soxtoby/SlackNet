using System;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet;

public abstract class FactorySlackServiceConfigurationWithDependencyResolver<TConfig, TDependencyResolver> : FactorySlackServiceConfiguration<TConfig> where TConfig : FactorySlackServiceConfigurationWithDependencyResolver<TConfig, TDependencyResolver>
{
    public TConfig UseHttp(Func<TDependencyResolver, IHttp> httpProvider) => UseHttp(GetServiceFactory(httpProvider));
    public TConfig UseJsonSettings(Func<TDependencyResolver, SlackJsonSettings> jsonSettingsProvider) => UseJsonSettings(GetServiceFactory(jsonSettingsProvider));
    public TConfig UseTypeResolver(Func<TDependencyResolver, ISlackTypeResolver> slackTypeResolverProvider) => UseTypeResolver(GetServiceFactory(slackTypeResolverProvider));
    public TConfig UseUrlBuilder(Func<TDependencyResolver, ISlackUrlBuilder> urlBuilderProvider) => UseUrlBuilder(GetServiceFactory(urlBuilderProvider));
    public TConfig UseLogger(Func<TDependencyResolver, ILogger> urlBuilderProvider) => UseLogger(GetServiceFactory(urlBuilderProvider));
    public TConfig UseWebSocketFactory(Func<TDependencyResolver, IWebSocketFactory> webSocketFactoryProvider) => UseWebSocketFactory(GetServiceFactory(webSocketFactoryProvider));
    public TConfig UseRequestListener(Func<TDependencyResolver, ISlackRequestListener> requestListenerProvider) => UseRequestListener(GetServiceFactory(requestListenerProvider));
    public TConfig UseHandlerFactory(Func<TDependencyResolver, ISlackHandlerFactory> handlerFactoryProvider) => UseHandlerFactory(GetServiceFactory(handlerFactoryProvider));
    public TConfig UseApiClient(Func<TDependencyResolver, ISlackApiClient> apiClientProvider) => UseApiClient(GetServiceFactory(apiClientProvider));
    public TConfig UseSocketModeClient(Func<TDependencyResolver, ISlackSocketModeClient> socketModeClientProvider) => UseSocketModeClient(GetServiceFactory(socketModeClientProvider));

    public TConfig ReplaceEventHandling(Func<TDependencyResolver, IEventHandler> handlerFactory) => ReplaceEventHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceBlockActionHandling(Func<TDependencyResolver, IAsyncBlockActionHandler> handlerFactory) => ReplaceBlockActionHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceBlockOptionProviding(Func<TDependencyResolver, IBlockOptionProvider> providerFactory) => ReplaceBlockOptionProviding(GetRequestHandlerFactory(providerFactory));
    public TConfig ReplaceMessageShortcutHandling(Func<TDependencyResolver, IAsyncMessageShortcutHandler> handlerFactory) => ReplaceMessageShortcutHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceGlobalShortcutHandling(Func<TDependencyResolver, IAsyncGlobalShortcutHandler> handlerFactory) => ReplaceGlobalShortcutHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceViewSubmissionHandling(Func<TDependencyResolver, IAsyncViewSubmissionHandler> handlerFactory) => ReplaceViewSubmissionHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceSlashCommandHandling(Func<TDependencyResolver, IAsyncSlashCommandHandler> handlerFactory) => ReplaceSlashCommandHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceWorkflowStepEditHandling(Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) => ReplaceWorkflowStepEditHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceLegacyInteractiveMessageHandling(Func<TDependencyResolver, IInteractiveMessageHandler> handlerFactory) => ReplaceLegacyInteractiveMessageHandling(GetRequestHandlerFactory(handlerFactory));
    public TConfig ReplaceLegacyOptionProviding(Func<TDependencyResolver, IOptionProvider> providerFactory) => ReplaceLegacyOptionProviding(GetRequestHandlerFactory(providerFactory));
    public TConfig ReplaceLegacyDialogSubmissionHandling(Func<TDependencyResolver, IDialogSubmissionHandler> handlerFactory) => ReplaceLegacyDialogSubmissionHandling(GetRequestHandlerFactory(handlerFactory));

    public TConfig RegisterEventHandler<TEvent>(Func<TDependencyResolver, IEventHandler<TEvent>> handlerFactory) where TEvent : Event =>
        RegisterEventHandler(r => handlerFactory(r).ToEventHandler());

    public TConfig RegisterEventHandler(Func<TDependencyResolver, IEventHandler> getHandler) =>
        RegisterEventHandler(GetRequestHandlerFactory(getHandler));

    public TConfig RegisterBlockActionHandler<TAction>(string actionId, Func<TDependencyResolver, IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterBlockActionHandler(actionId, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterBlockActionHandler<TAction>(Func<TDependencyResolver, IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterBlockActionHandler(GetRequestHandlerFactory(getHandler));

    public TConfig RegisterBlockActionHandler(Func<TDependencyResolver, IBlockActionHandler> getHandler) =>
        RegisterBlockActionHandler(GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<TDependencyResolver, IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(actionId, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(Func<TDependencyResolver, IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler(Func<TDependencyResolver, IAsyncBlockActionHandler> getHandler) =>
        RegisterAsyncBlockActionHandler(GetRequestHandlerFactory(getHandler));

    public TConfig RegisterBlockOptionProvider(string actionId, Func<TDependencyResolver, IBlockOptionProvider> getProvider) =>
        RegisterBlockOptionProvider(actionId, GetRequestHandlerFactory(getProvider));

    public TConfig RegisterMessageShortcutHandler(string callbackId, Func<TDependencyResolver, IMessageShortcutHandler> getHandler) =>
        RegisterMessageShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterMessageShortcutHandler(Func<TDependencyResolver, IMessageShortcutHandler> getHandler) =>
        RegisterMessageShortcutHandler(GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, Func<TDependencyResolver, IAsyncMessageShortcutHandler> getHandler) =>
        RegisterAsyncMessageShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(Func<TDependencyResolver, IAsyncMessageShortcutHandler> getHandler) =>
        RegisterAsyncMessageShortcutHandler(GetRequestHandlerFactory(getHandler));

    public TConfig RegisterGlobalShortcutHandler(string callbackId, Func<TDependencyResolver, IGlobalShortcutHandler> getHandler) =>
        RegisterGlobalShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterGlobalShortcutHandler(Func<TDependencyResolver, IGlobalShortcutHandler> getHandler) =>
        RegisterGlobalShortcutHandler(GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, Func<TDependencyResolver, IAsyncGlobalShortcutHandler> getHandler) =>
        RegisterAsyncGlobalShortcutHandler(callbackId, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(Func<TDependencyResolver, IAsyncGlobalShortcutHandler> getHandler) =>
        RegisterAsyncGlobalShortcutHandler(GetRequestHandlerFactory(getHandler));

    public TConfig RegisterViewSubmissionHandler(string callbackId, Func<TDependencyResolver, IViewSubmissionHandler> getHandler) =>
        RegisterViewSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, Func<TDependencyResolver, IAsyncViewSubmissionHandler> getHandler) =>
        RegisterAsyncViewSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterSlashCommandHandler(string command, Func<TDependencyResolver, ISlashCommandHandler> getHandler) =>
        RegisterSlashCommandHandler(command, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncSlashCommandHandler(string command, Func<TDependencyResolver, IAsyncSlashCommandHandler> getHandler) =>
        RegisterAsyncSlashCommandHandler(command, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterWorkflowStepEditHandler(string callbackId, Func<TDependencyResolver, IWorkflowStepEditHandler> getHandler) =>
        RegisterWorkflowStepEditHandler(callbackId, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterWorkflowStepEditHandler(Func<TDependencyResolver, IWorkflowStepEditHandler> getHandler) =>
        RegisterWorkflowStepEditHandler(GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncWorkflowStepEditHandler(string callbackId, Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> getHandler) =>
        RegisterAsyncWorkflowStepEditHandler(callbackId, GetRequestHandlerFactory(getHandler));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncWorkflowStepEditHandler(Func<TDependencyResolver, IAsyncWorkflowStepEditHandler> handlerFactory) =>
        RegisterAsyncWorkflowStepEditHandler(GetRequestHandlerFactory(handlerFactory));

    public TConfig RegisterInteractiveMessageHandler(string actionName, Func<TDependencyResolver, IInteractiveMessageHandler> getHandler) =>
        RegisterInteractiveMessageHandler(actionName, GetRequestHandlerFactory(getHandler));

    public TConfig RegisterOptionProvider(string actionName, Func<TDependencyResolver, IOptionProvider> getProvider) =>
        RegisterOptionProvider(actionName, GetRequestHandlerFactory(getProvider));

    public TConfig RegisterDialogSubmissionHandler(string callbackId, Func<TDependencyResolver, IDialogSubmissionHandler> getHandler) =>
        RegisterDialogSubmissionHandler(callbackId, GetRequestHandlerFactory(getHandler));

    /// <summary>
    /// Get a service factory for the given service callback. The service will be created once.
    /// </summary>
    protected abstract Func<ISlackServiceProvider, TService> GetServiceFactory<TService>(Func<TDependencyResolver, TService> getService) where TService : class;

    /// <summary>
    /// Get a factory for creating a handler for a request. The handler will be created once per request.
    /// </summary>
    protected abstract Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<TDependencyResolver, THandler> getHandler) where THandler : class;
}