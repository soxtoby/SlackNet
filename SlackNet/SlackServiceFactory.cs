using System;
using System.Collections.Generic;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;

namespace SlackNet
{
    /// <summary>
    /// A basic factory for SlackNet services, with some configuration.
    /// If you're using a dependency injection library, you're probably better off integrating with that instead of using this.
    /// </summary>
    public class SlackServiceFactory : SlackHandlerConfigurationBase<SlackServiceFactory>, ISlackServiceFactory
    {
        private Lazy<IHttp> _http;
        private Lazy<SlackJsonSettings> _jsonSettings;
        private Lazy<ISlackTypeResolver> _slackTypeResolver;
        private Lazy<ISlackUrlBuilder> _slackUrlBuilder;
        private Lazy<IWebSocketFactory> _webSocketFactory;
        private Lazy<ISlackRequestListener> _requestListener;
        private Lazy<ISlackHandlerFactory> _handlerFactory;
        private Lazy<ISlackApiClient> _slackApiClient;
        private Lazy<ISlackSocketModeClient> _socketModeClient;

        private CollectionHandlerFactory<IEventHandler> _eventHandlerFactory;
        private CollectionHandlerFactory<IAsyncBlockActionHandler> _blockActionHandlerFactory;
        private KeyedHandlerFactory<IBlockOptionProvider> _blockOptionProviderFactory;
        private CollectionHandlerFactory<IAsyncMessageShortcutHandler> _messageShortcutHandlerFactory;
        private CollectionHandlerFactory<IAsyncGlobalShortcutHandler> _globalShortcutHandlerFactory;
        private KeyedHandlerFactory<IAsyncViewSubmissionHandler> _viewSubmissionHandlerFactory;
        private KeyedHandlerFactory<IAsyncSlashCommandHandler> _slashCommandHandlerFactory;
        private CollectionHandlerFactory<IAsyncWorkflowStepEditHandler> _workflowStepEditHandlerFactory;

        private KeyedHandlerFactory<IInteractiveMessageHandler> _legacyInteractiveMessageHandlerFactory;
        private KeyedHandlerFactory<IOptionProvider> _legacyOptionProviderFactory;
        private KeyedHandlerFactory<IDialogSubmissionHandler> _legacyDialogSubmissionHandlerFactory;

        private readonly List<IEventHandler> _eventHandlers = new();
        private readonly List<IAsyncBlockActionHandler> _blockActionHandlers = new();
        private readonly Dictionary<string, IBlockOptionProvider> _blockOptionProviders = new();
        private readonly List<IAsyncMessageShortcutHandler> _messageShortcutHandlers = new();
        private readonly List<IAsyncGlobalShortcutHandler> _globalShortcutHandlers = new();
        private readonly Dictionary<string, IAsyncViewSubmissionHandler> _viewSubmissionHandlers = new();
        private readonly Dictionary<string, IAsyncSlashCommandHandler> _slashCommandHandlers = new();
        private readonly List<IAsyncWorkflowStepEditHandler> _workflowStepEditHandlers = new();

        private readonly Dictionary<string, IInteractiveMessageHandler> _legacyInteractiveMessageHandlers = new();
        private readonly Dictionary<string, IOptionProvider> _legacyOptionProviders = new();
        private readonly Dictionary<string, IDialogSubmissionHandler> _legacyDialogSubmissionHandlers = new();

        public SlackServiceFactory()
        {
            _http = new Lazy<IHttp>(() => Default.Http(GetJsonSettings()));
            _jsonSettings = new Lazy<SlackJsonSettings>(() => Default.JsonSettings(GetTypeResolver()));
            _slackTypeResolver = new Lazy<ISlackTypeResolver>(Default.SlackTypeResolver);
            _webSocketFactory = new Lazy<IWebSocketFactory>(() => Default.WebSocketFactory);
            _requestListener = new Lazy<ISlackRequestListener>(() => Default.RequestListener);
            _slackUrlBuilder = new Lazy<ISlackUrlBuilder>(() => Default.UrlBuilder(GetJsonSettings()));
            _handlerFactory = new Lazy<ISlackHandlerFactory>(() => new SlackHandlerFactory(
                () => _eventHandlerFactory(_eventHandlers),
                () => _blockActionHandlerFactory(_blockActionHandlers),
                () => _blockOptionProviderFactory(new HandlerDictionary<IBlockOptionProvider>(_blockOptionProviders)),
                () => _messageShortcutHandlerFactory(_messageShortcutHandlers),
                () => _globalShortcutHandlerFactory(_globalShortcutHandlers),
                () => _viewSubmissionHandlerFactory(new HandlerDictionary<IAsyncViewSubmissionHandler>(_viewSubmissionHandlers)),
                () => _slashCommandHandlerFactory(new HandlerDictionary<IAsyncSlashCommandHandler>(_slashCommandHandlers)),
                () => _workflowStepEditHandlerFactory(_workflowStepEditHandlers),
                () => _legacyInteractiveMessageHandlerFactory(new HandlerDictionary<IInteractiveMessageHandler>(_legacyInteractiveMessageHandlers)),
                () => _legacyOptionProviderFactory(new HandlerDictionary<IOptionProvider>(_legacyOptionProviders)),
                () => _legacyDialogSubmissionHandlerFactory(new HandlerDictionary<IDialogSubmissionHandler>(_legacyDialogSubmissionHandlers))
            ));

             _eventHandlerFactory = hs => new CompositeEventHandler(hs);
             _blockActionHandlerFactory = hs => new CompositeBlockActionHandler(hs);
             _blockOptionProviderFactory = hs => new SwitchingBlockOptionProvider(hs);
             _messageShortcutHandlerFactory = hs => new CompositeMessageShortcutHandler(hs);
             _globalShortcutHandlerFactory = hs => new CompositeGlobalShortcutHandler(hs);
             _viewSubmissionHandlerFactory = hs => new SwitchingViewSubmissionHandler(hs);
             _slashCommandHandlerFactory = hs => new SwitchingSlashCommandHandler(hs);
             _workflowStepEditHandlerFactory = hs => new CompositeWorkflowStepEditHandler(hs);
             _legacyInteractiveMessageHandlerFactory = hs => new SwitchingInteractiveMessageHandler(hs);
             _legacyOptionProviderFactory = hs => new SwitchingOptionProvider(hs);
             _legacyDialogSubmissionHandlerFactory = hs => new SwitchingDialogSubmissionHandler(hs);

            _slackApiClient = new Lazy<ISlackApiClient>(() => new SlackApiClient(GetHttp(), GetUrlBuilder(), GetJsonSettings(), ApiToken));
            _socketModeClient = new Lazy<ISlackSocketModeClient>(() =>
                new SlackSocketModeClient(
                    new CoreSocketModeClient(
                        GetApiClient().WithAccessToken(AppLevelToken),
                        GetWebSocketFactory(),
                        GetJsonSettings(),
                        Default.Scheduler),
                    GetJsonSettings(),
                    GetRequestListener(),
                    GetHandlerFactory()));
        }

        public override SlackServiceFactory UseHttp(Func<IHttp> httpProvider) => Chain(() => _http = new Lazy<IHttp>(httpProvider));
        public override SlackServiceFactory UseJsonSettings(Func<SlackJsonSettings> jsonSettingsProvider) => Chain(() => _jsonSettings = new Lazy<SlackJsonSettings>(jsonSettingsProvider));
        public override SlackServiceFactory UseTypeResolver(Func<ISlackTypeResolver> slackTypeResolverProvider) => Chain(() => _slackTypeResolver = new Lazy<ISlackTypeResolver>(slackTypeResolverProvider));
        public override SlackServiceFactory UseUrlBuilder(Func<ISlackUrlBuilder> urlBuilderProvider) => Chain(() => _slackUrlBuilder = new Lazy<ISlackUrlBuilder>(urlBuilderProvider));
        public override SlackServiceFactory UseWebSocketFactory(Func<IWebSocketFactory> webSocketFactoryProvider) => Chain(() => _webSocketFactory = new Lazy<IWebSocketFactory>(webSocketFactoryProvider));
        public override SlackServiceFactory UseRequestListener(Func<ISlackRequestListener> requestListenerProvider) => Chain(() => _requestListener = new Lazy<ISlackRequestListener>(requestListenerProvider));
        public override SlackServiceFactory UseHandlerFactory(Func<ISlackHandlerFactory> handlerFactoryProvider) => Chain(() => _handlerFactory = new Lazy<ISlackHandlerFactory>(handlerFactoryProvider));
        public override SlackServiceFactory UseApiClient(Func<ISlackApiClient> apiClientProvider) => Chain(() => _slackApiClient = new Lazy<ISlackApiClient>(apiClientProvider));
        public override SlackServiceFactory UseSocketModeClient(Func<ISlackSocketModeClient> socketModeClientProvider) => Chain(() => _socketModeClient = new Lazy<ISlackSocketModeClient>(socketModeClientProvider));

        public override SlackServiceFactory ReplaceEventHandling(CollectionHandlerFactory<IEventHandler> handlerFactory) => Chain(() => _eventHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceBlockActionHandling(CollectionHandlerFactory<IAsyncBlockActionHandler> handlerFactory) => Chain(() => _blockActionHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceBlockOptionProviding(KeyedHandlerFactory<IBlockOptionProvider> providerFactory) => Chain(() => _blockOptionProviderFactory = providerFactory);
        public override SlackServiceFactory ReplaceMessageShortcutHandling(CollectionHandlerFactory<IAsyncMessageShortcutHandler> handlerFactory) => Chain(() => _messageShortcutHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceGlobalShortcutHandling(CollectionHandlerFactory<IAsyncGlobalShortcutHandler> handlerFactory) => Chain(() => _globalShortcutHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceViewSubmissionHandling(KeyedHandlerFactory<IAsyncViewSubmissionHandler> handlerFactory) => Chain(() => _viewSubmissionHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceSlashCommandHandling(KeyedHandlerFactory<IAsyncSlashCommandHandler> handlerFactory) => Chain(() => _slashCommandHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceWorkflowStepEditHandling(CollectionHandlerFactory<IAsyncWorkflowStepEditHandler> handlerFactory) => Chain(() => _workflowStepEditHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceLegacyInteractiveMessageHandling(KeyedHandlerFactory<IInteractiveMessageHandler> handlerFactory) => Chain(() => _legacyInteractiveMessageHandlerFactory = handlerFactory);
        public override SlackServiceFactory ReplaceLegacyOptionProviding(KeyedHandlerFactory<IOptionProvider> providerFactory) => Chain(() => _legacyOptionProviderFactory = providerFactory);
        public override SlackServiceFactory ReplaceLegacyDialogSubmissionHandling(KeyedHandlerFactory<IDialogSubmissionHandler> handlerFactory) => Chain(() => _legacyDialogSubmissionHandlerFactory = handlerFactory);

        protected override void AddEventHandler(IEventHandler handler) => _eventHandlers.Add(handler);
        protected override void AddBlockActionHandler(IAsyncBlockActionHandler handler) => _blockActionHandlers.Add(handler);
        protected override void AddBlockOptionProvider(string actionId, IBlockOptionProvider provider) => _blockOptionProviders.Add(actionId, provider);
        protected override void AddMessageShortcutHandler(IAsyncMessageShortcutHandler handler) => _messageShortcutHandlers.Add(handler);
        protected override void AddGlobalShortcutHandler(IAsyncGlobalShortcutHandler handler) => _globalShortcutHandlers.Add(handler);
        protected override void AddViewSubmissionHandler(string callbackId, IAsyncViewSubmissionHandler handler) => _viewSubmissionHandlers.Add(callbackId, handler);
        protected override void AddSlashCommandHandler(string command, IAsyncSlashCommandHandler handler) => _slashCommandHandlers.Add(command, handler);
        protected override void AddWorkflowStepEditHandler(IAsyncWorkflowStepEditHandler handler) => _workflowStepEditHandlers.Add(handler);

        protected override void AddLegacyInteractiveMessageHandler(string actionName, IInteractiveMessageHandler handler) => ((IDictionary<string, IInteractiveMessageHandler>)_legacyInteractiveMessageHandlers).Add(actionName, handler);
        protected override void AddLegacyOptionProvider(string actionName, IOptionProvider provider) => ((IDictionary<string, IOptionProvider>)_legacyOptionProviders).Add(actionName, provider);
        protected override void AddLegacyDialogSubmissionHandler(string callbackId, IDialogSubmissionHandler handler) => ((IDictionary<string, IDialogSubmissionHandler>)_legacyDialogSubmissionHandlers).Add(callbackId, handler);

        public IHttp GetHttp() => _http.Value;
        public SlackJsonSettings GetJsonSettings() => _jsonSettings.Value;
        public ISlackTypeResolver GetTypeResolver() => _slackTypeResolver.Value;
        public ISlackUrlBuilder GetUrlBuilder() => _slackUrlBuilder.Value;
        public IWebSocketFactory GetWebSocketFactory() => _webSocketFactory.Value;
        public ISlackRequestListener GetRequestListener() => _requestListener.Value;
        public ISlackHandlerFactory GetHandlerFactory() => _handlerFactory.Value;
        public ISlackApiClient GetApiClient() => _slackApiClient.Value;
        public ISlackSocketModeClient GetSocketModeClient() => _socketModeClient.Value;
    }
}
