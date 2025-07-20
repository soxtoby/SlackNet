using System;
using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;

namespace SlackNet;

public abstract class SlackServiceConfigurationBase<TConfig> where TConfig : SlackServiceConfigurationBase<TConfig>
{
    private string _apiToken;
    private string _appLevelToken;

    private Func<ISlackServiceProvider, IHttp> _httpProvider = sp => Default.Http(sp.GetJsonSettings(), logger: sp.GetLogger());
    private Func<ISlackServiceProvider, SlackJsonSettings> _jsonSettingsProvider = sp => Default.JsonSettings(sp.GetTypeResolver(), sp.GetLogger());
    private Func<ISlackServiceProvider, ISlackTypeResolver> _slackTypeResolverProvider = sp => Default.SlackTypeResolver();
    private Func<ISlackServiceProvider, ISlackUrlBuilder> _urlBuilderProvider = sp => Default.UrlBuilder(sp.GetJsonSettings());
    private Func<ISlackServiceProvider, ILogger> _loggerProvider = sp => Default.Logger;
    private Func<ISlackServiceProvider, IWebSocketFactory> _webSocketFactoryProvider = sp => Default.WebSocketFactory;
    private readonly List<Func<ISlackServiceProvider, ISlackRequestListener>> _requestListenerProviders = new();
    private Func<ISlackServiceProvider, ISlackHandlerFactory> _handlerFactoryProvider;
    private Func<ISlackServiceProvider, ISlackApiClient> _apiClientProvider;
    private Func<ISlackServiceProvider, ISlackSocketModeClient> _socketModeClientProvider;

    private readonly List<Func<SlackRequestContext, IEventHandler>> _eventHandlers = new();
    private readonly List<Func<SlackRequestContext, IAsyncBlockActionHandler>> _blockActionHandlers = new();
    private readonly Dictionary<string, Func<SlackRequestContext, IBlockOptionProvider>> _blockOptionProviders = new();
    private readonly List<Func<SlackRequestContext, IAsyncMessageShortcutHandler>> _messageShortcutHandlers = new();
    private readonly List<Func<SlackRequestContext, IAsyncGlobalShortcutHandler>> _globalShortcutHandlers = new();
    private readonly Dictionary<string, Func<SlackRequestContext, IAsyncViewSubmissionHandler>> _viewSubmissionHandlers = new();
    private readonly Dictionary<string, Func<SlackRequestContext, IAsyncSlashCommandHandler>> _slashCommandHandlers = new();

    private readonly Dictionary<string, Func<SlackRequestContext, IInteractiveMessageHandler>> _legacyInteractiveMessageHandlers = new();
    private readonly Dictionary<string, Func<SlackRequestContext, IOptionProvider>> _legacyOptionProviders = new();
    private readonly Dictionary<string, Func<SlackRequestContext, IDialogSubmissionHandler>> _legacyDialogSubmissionHandlers = new();

    private Func<SlackRequestContext, IEventHandler> _eventHandlerFactory = ctx => new CompositeEventHandler(ctx.EventHandlers);
    private Func<SlackRequestContext, IAsyncBlockActionHandler> _blockActionHandlerFactory = ctx => new CompositeBlockActionHandler(ctx.BlockActionHandlers);
    private Func<SlackRequestContext, IBlockOptionProvider> _blockOptionProviderFactory = ctx => new SwitchingBlockOptionProvider(ctx.BlockOptionProviders);
    private Func<SlackRequestContext, IAsyncMessageShortcutHandler> _messageShortcutHandlerFactory = ctx => new CompositeMessageShortcutHandler(ctx.MessageShortcutHandlers);
    private Func<SlackRequestContext, IAsyncGlobalShortcutHandler> _globalShortcutHandlerFactory = ctx => new CompositeGlobalShortcutHandler(ctx.GlobalShortcutHandlers);
    private Func<SlackRequestContext, IAsyncViewSubmissionHandler> _viewSubmissionHandlerFactory = ctx => new SwitchingViewSubmissionHandler(ctx.ViewSubmissionHandlers);
    private Func<SlackRequestContext, IAsyncSlashCommandHandler> _slashCommandHandlerFactory = ctx => new SwitchingSlashCommandHandler(ctx.SlashCommandHandlers);

    private Func<SlackRequestContext, IInteractiveMessageHandler> _legacyInteractiveMessageHandlerFactory = ctx => new SwitchingInteractiveMessageHandler(ctx.LegacyInteractiveMessageHandlers);
    private Func<SlackRequestContext, IOptionProvider> _legacyOptionProviderFactory = ctx => new SwitchingOptionProvider(ctx.LegacyOptionProviders);
    private Func<SlackRequestContext, IDialogSubmissionHandler> _legacyDialogSubmissionHandlerFactory = ctx => new SwitchingDialogSubmissionHandler(ctx.LegacyDialogSubmissionHandlers);

    protected SlackServiceConfigurationBase()
    {
        _requestListenerProviders.Add(CreateRequestContextPopulator);
        _handlerFactoryProvider = CreateHandlerFactory;
        _apiClientProvider = CreateApiClient;
        _socketModeClientProvider = CreateSocketModeClient;
    }

    /// <summary>
    /// Used for making Slack API requests. Can be either a bot or user token.
    /// </summary>
    public TConfig UseApiToken(string token) => Chain(() => _apiToken = token);

    /// <summary>
    /// Used for connection socket mode client.
    /// </summary>
    public TConfig UseAppLevelToken(string token) => Chain(() => _appLevelToken = token);

    public TConfig UseHttp(Func<ISlackServiceProvider, IHttp> httpProvider) => Chain(() => _httpProvider = httpProvider);
    public TConfig UseJsonSettings(Func<ISlackServiceProvider, SlackJsonSettings> jsonSettingsProvider) => Chain(() => _jsonSettingsProvider = jsonSettingsProvider);
    public TConfig UseTypeResolver(Func<ISlackServiceProvider, ISlackTypeResolver> slackTypeResolverProvider) => Chain(() => _slackTypeResolverProvider = slackTypeResolverProvider);
    public TConfig UseUrlBuilder(Func<ISlackServiceProvider, ISlackUrlBuilder> urlBuilderProvider) => Chain(() => _urlBuilderProvider = urlBuilderProvider);
    public TConfig UseLogger(Func<ISlackServiceProvider, ILogger> loggerProvider) => Chain(() => _loggerProvider = loggerProvider);
    public TConfig UseWebSocketFactory(Func<ISlackServiceProvider, IWebSocketFactory> webSocketFactoryProvider) => Chain(() => _webSocketFactoryProvider = webSocketFactoryProvider);
    public TConfig UseRequestListener(Func<ISlackServiceProvider, ISlackRequestListener> requestListenerProvider) => Chain(() => _requestListenerProviders.Add(requestListenerProvider));
    public TConfig UseHandlerFactory(Func<ISlackServiceProvider, ISlackHandlerFactory> handlerFactoryProvider) => Chain(() => _handlerFactoryProvider = handlerFactoryProvider);
    public TConfig UseApiClient(Func<ISlackServiceProvider, ISlackApiClient> apiClientProvider) => Chain(() => _apiClientProvider = apiClientProvider);
    public TConfig UseSocketModeClient(Func<ISlackServiceProvider, ISlackSocketModeClient> socketModeClientProvider) => Chain(() => _socketModeClientProvider = socketModeClientProvider);

    protected ISlackServiceProvider CreateServiceFactory(ISlackServiceProvider dependencyProvider = null) =>
        new SlackServiceProvider(
            _httpProvider,
            _jsonSettingsProvider,
            _slackTypeResolverProvider,
            _urlBuilderProvider,
            _loggerProvider,
            _webSocketFactoryProvider,
            _requestListenerProviders,
            _handlerFactoryProvider,
            _apiClientProvider,
            _socketModeClientProvider,
            dependencyProvider);

    ISlackHandlerFactory CreateHandlerFactory(ISlackServiceProvider serviceProvider) =>
        new SlackHandlerFactory(
            _eventHandlerFactory,
            _blockActionHandlerFactory,
            _blockOptionProviderFactory,
            _messageShortcutHandlerFactory,
            _globalShortcutHandlerFactory,
            _viewSubmissionHandlerFactory,
            _slashCommandHandlerFactory,
            _legacyInteractiveMessageHandlerFactory,
            _legacyOptionProviderFactory,
            _legacyDialogSubmissionHandlerFactory);

    ISlackApiClient CreateApiClient(ISlackServiceProvider serviceProvider) =>
        new SlackApiClient(
            serviceProvider.GetHttp(),
            serviceProvider.GetUrlBuilder(),
            serviceProvider.GetJsonSettings(),
            _apiToken);

    ISlackSocketModeClient CreateSocketModeClient(ISlackServiceProvider serviceProvider) =>
        new SlackSocketModeClient(
            new CoreSocketModeClient(
                serviceProvider.GetApiClient().WithAccessToken(_appLevelToken),
                serviceProvider.GetWebSocketFactory(),
                serviceProvider.GetJsonSettings(),
                Default.Scheduler,
                serviceProvider.GetLogger()),
            serviceProvider.GetJsonSettings(),
            serviceProvider.GetRequestListeners(),
            serviceProvider.GetHandlerFactory(),
            serviceProvider.GetLogger());

    private ISlackRequestListener CreateRequestContextPopulator(ISlackServiceProvider serviceProvider) =>
        new SlackRequestContextPopulator(
            serviceProvider,
            _eventHandlers,
            _blockActionHandlers,
            _blockOptionProviders,
            _messageShortcutHandlers,
            _globalShortcutHandlers,
            _viewSubmissionHandlers,
            _slashCommandHandlers,
            _legacyInteractiveMessageHandlers,
            _legacyOptionProviders,
            _legacyDialogSubmissionHandlers);

    /// <summary>
    /// Take over all event handling with your own handler.
    /// </summary>
    public TConfig ReplaceEventHandling(Func<SlackRequestContext, IEventHandler> handlerFactory) => Chain(() => _eventHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all block action handling with your own handler.
    /// </summary>
    public TConfig ReplaceBlockActionHandling(Func<SlackRequestContext, IAsyncBlockActionHandler> handlerFactory) => Chain(() => _blockActionHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all block option providing with your own provider.
    /// </summary>
    public TConfig ReplaceBlockOptionProviding(Func<SlackRequestContext, IBlockOptionProvider> providerFactory) => Chain(() => _blockOptionProviderFactory = providerFactory);

    /// <summary>
    /// Take over all message shortcut handling with your own handler.
    /// </summary>
    public TConfig ReplaceMessageShortcutHandling(Func<SlackRequestContext, IAsyncMessageShortcutHandler> handlerFactory) => Chain(() => _messageShortcutHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all global shortcut handling with your own handler.
    /// </summary>
    public TConfig ReplaceGlobalShortcutHandling(Func<SlackRequestContext, IAsyncGlobalShortcutHandler> handlerFactory) => Chain(() => _globalShortcutHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all view submission handling with your own handler.
    /// </summary>
    public TConfig ReplaceViewSubmissionHandling(Func<SlackRequestContext, IAsyncViewSubmissionHandler> handlerFactory) => Chain(() => _viewSubmissionHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all slash command handling with your own handler.
    /// </summary>
    public TConfig ReplaceSlashCommandHandling(Func<SlackRequestContext, IAsyncSlashCommandHandler> handlerFactory) => Chain(() => _slashCommandHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all interactive message handling with your own handler.
    /// </summary>
    public TConfig ReplaceLegacyInteractiveMessageHandling(Func<SlackRequestContext, IInteractiveMessageHandler> handlerFactory) => Chain(() => _legacyInteractiveMessageHandlerFactory = handlerFactory);

    /// <summary>
    /// Take over all legacy option providing with your own provider.
    /// </summary>
    public TConfig ReplaceLegacyOptionProviding(Func<SlackRequestContext, IOptionProvider> providerFactory) => Chain(() => _legacyOptionProviderFactory = providerFactory);

    /// <summary>
    /// Take over all legacy dialog submission handling with your own handler.
    /// </summary>
    public TConfig ReplaceLegacyDialogSubmissionHandling(Func<SlackRequestContext, IDialogSubmissionHandler> handlerFactory) => Chain(() => _legacyDialogSubmissionHandlerFactory = handlerFactory);

    public TConfig RegisterEventHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : Event =>
        RegisterEventHandler(_ => handler);

    public TConfig RegisterEventHandler(IEventHandler eventHandler) =>
        RegisterEventHandler(_ => eventHandler);

    public TConfig RegisterBlockActionHandler<TAction>(string actionId, IBlockActionHandler<TAction> handler) where TAction : BlockAction =>
        RegisterBlockActionHandler(actionId, _ => handler);

    public TConfig RegisterBlockActionHandler<TAction>(IBlockActionHandler<TAction> handler) where TAction : BlockAction =>
        RegisterBlockActionHandler(_ => handler);

    public TConfig RegisterBlockActionHandler(IBlockActionHandler handler) =>
        RegisterBlockActionHandler(_ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, IAsyncBlockActionHandler<TAction> handler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(actionId, _ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(IAsyncBlockActionHandler<TAction> handler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(_ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler(IAsyncBlockActionHandler handler) =>
        RegisterAsyncBlockActionHandler(_ => handler);

    public TConfig RegisterBlockOptionProvider(string actionId, IBlockOptionProvider provider) =>
        RegisterBlockOptionProvider(actionId, _ => provider);

    public TConfig RegisterMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler) =>
        RegisterMessageShortcutHandler(callbackId, _ => handler);

    public TConfig RegisterMessageShortcutHandler(IMessageShortcutHandler handler) =>
        RegisterMessageShortcutHandler(_ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, IAsyncMessageShortcutHandler handler) =>
        RegisterAsyncMessageShortcutHandler(callbackId, _ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(IAsyncMessageShortcutHandler handler) =>
        RegisterAsyncMessageShortcutHandler(_ => handler);

    public TConfig RegisterGlobalShortcutHandler(string callbackId, IGlobalShortcutHandler handler) =>
        RegisterGlobalShortcutHandler(callbackId, _ => handler);

    public TConfig RegisterGlobalShortcutHandler(IGlobalShortcutHandler handler) =>
        RegisterGlobalShortcutHandler(_ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, IAsyncGlobalShortcutHandler handler) =>
        RegisterAsyncGlobalShortcutHandler(callbackId, _ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler) =>
        RegisterAsyncGlobalShortcutHandler(_ => handler);

    public TConfig RegisterViewSubmissionHandler(string callbackId, IViewSubmissionHandler handler) =>
        RegisterViewSubmissionHandler(callbackId, _ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler) =>
        RegisterAsyncViewSubmissionHandler(callbackId, _ => handler);

    public TConfig RegisterSlashCommandHandler(string command, ISlashCommandHandler handler) =>
        RegisterSlashCommandHandler(command, _ => handler);

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncSlashCommandHandler(string command, IAsyncSlashCommandHandler handler) =>
        RegisterAsyncSlashCommandHandler(command, _ => handler);

    public TConfig RegisterInteractiveMessageHandler(string actionName, IInteractiveMessageHandler handler) =>
        RegisterInteractiveMessageHandler(actionName, _ => handler);

    public TConfig RegisterOptionProvider(string actionName, IOptionProvider provider) =>
        RegisterOptionProvider(actionName, _ => provider);

    public TConfig RegisterDialogSubmissionHandler(string callbackId, IDialogSubmissionHandler handler) =>
        RegisterDialogSubmissionHandler(callbackId, _ => handler);

    public TConfig RegisterEventHandler<TEvent>(Func<SlackRequestContext, IEventHandler<TEvent>> getHandler) where TEvent : Event =>
        RegisterEventHandler(ctx => getHandler(ctx).ToEventHandler());

    public TConfig RegisterEventHandler(Func<SlackRequestContext, IEventHandler> getEventHandler) =>
        Chain(() => _eventHandlers.Add(getEventHandler));

    public TConfig RegisterBlockActionHandler<TAction>(string actionId, Func<SlackRequestContext, IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(ctx => getHandler(ctx).ToBlockActionHandler(actionId));

    public TConfig RegisterBlockActionHandler<TAction>(Func<SlackRequestContext, IBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(ctx => getHandler(ctx).ToBlockActionHandler());

    public TConfig RegisterBlockActionHandler(Func<SlackRequestContext, IBlockActionHandler> getHandler) =>
        RegisterAsyncBlockActionHandler(ctx => getHandler(ctx).ToBlockActionHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(string actionId, Func<SlackRequestContext, IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(ctx => getHandler(ctx).ToBlockActionHandler(actionId));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler<TAction>(Func<SlackRequestContext, IAsyncBlockActionHandler<TAction>> getHandler) where TAction : BlockAction =>
        RegisterAsyncBlockActionHandler(ctx => getHandler(ctx).ToBlockActionHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncBlockActionHandler(Func<SlackRequestContext, IAsyncBlockActionHandler> getHandler) =>
        Chain(() => _blockActionHandlers.Add(getHandler));

    public TConfig RegisterBlockOptionProvider(string actionId, Func<SlackRequestContext, IBlockOptionProvider> getProvider) =>
        Chain(() => _blockOptionProviders.Add(actionId, getProvider));

    public TConfig RegisterMessageShortcutHandler(string callbackId, Func<SlackRequestContext, IMessageShortcutHandler> getHandler) =>
        RegisterAsyncMessageShortcutHandler(callbackId, ctx => getHandler(ctx).ToMessageShortcutHandler());

    public TConfig RegisterMessageShortcutHandler(Func<SlackRequestContext, IMessageShortcutHandler> getHandler) =>
        RegisterAsyncMessageShortcutHandler(ctx => getHandler(ctx).ToMessageShortcutHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(string callbackId, Func<SlackRequestContext, IAsyncMessageShortcutHandler> getHandler) =>
        RegisterAsyncMessageShortcutHandler(ctx => getHandler(ctx).ToMessageShortcutHandler(callbackId));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncMessageShortcutHandler(Func<SlackRequestContext, IAsyncMessageShortcutHandler> getHandler) =>
        Chain(() => _messageShortcutHandlers.Add(getHandler));

    public TConfig RegisterGlobalShortcutHandler(string callbackId, Func<SlackRequestContext, IGlobalShortcutHandler> getHandler) =>
        RegisterAsyncGlobalShortcutHandler(callbackId, ctx => getHandler(ctx).ToGlobalShortcutHandler());

    public TConfig RegisterGlobalShortcutHandler(Func<SlackRequestContext, IGlobalShortcutHandler> getHandler) =>
        RegisterAsyncGlobalShortcutHandler(ctx => getHandler(ctx).ToGlobalShortcutHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(string callbackId, Func<SlackRequestContext, IAsyncGlobalShortcutHandler> getHandler) =>
        RegisterAsyncGlobalShortcutHandler(ctx => getHandler(ctx).ToGlobalShortcutHandler(callbackId));

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncGlobalShortcutHandler(Func<SlackRequestContext, IAsyncGlobalShortcutHandler> getHandler) =>
        Chain(() => _globalShortcutHandlers.Add(getHandler));

    public TConfig RegisterViewSubmissionHandler(string callbackId, Func<SlackRequestContext, IViewSubmissionHandler> getHandler) =>
        RegisterAsyncViewSubmissionHandler(callbackId, ctx => getHandler(ctx).ToViewSubmissionHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncViewSubmissionHandler(string callbackId, Func<SlackRequestContext, IAsyncViewSubmissionHandler> getHandler) =>
        Chain(() => _viewSubmissionHandlers.Add(callbackId, getHandler));

    public TConfig RegisterSlashCommandHandler(string command, Func<SlackRequestContext, ISlashCommandHandler> getHandler) =>
        RegisterAsyncSlashCommandHandler(command, ctx => getHandler(ctx).ToSlashCommandHandler());

    [Obsolete(Warning.Experimental)]
    public TConfig RegisterAsyncSlashCommandHandler(string command, Func<SlackRequestContext, IAsyncSlashCommandHandler> getHandler)
    {
        ValidateCommandName(command);
        return Chain(() => _slashCommandHandlers.Add(command, getHandler));
    }

    private static void ValidateCommandName(string command)
    {
        if (!command.StartsWith("/"))
            throw new ArgumentException("Command must start with '/'", nameof(command));
    }

    public TConfig RegisterInteractiveMessageHandler(string actionName, Func<SlackRequestContext, IInteractiveMessageHandler> getHandler) =>
        Chain(() => _legacyInteractiveMessageHandlers.Add(actionName, getHandler));

    public TConfig RegisterOptionProvider(string actionName, Func<SlackRequestContext, IOptionProvider> getProvider) =>
        Chain(() => _legacyOptionProviders.Add(actionName, getProvider));

    public TConfig RegisterDialogSubmissionHandler(string callbackId, Func<SlackRequestContext, IDialogSubmissionHandler> getHandler) =>
        Chain(() => _legacyDialogSubmissionHandlers.Add(callbackId, getHandler));

    protected TConfig Chain(Action action)
    {
        action();
        return (TConfig)this;
    }
}