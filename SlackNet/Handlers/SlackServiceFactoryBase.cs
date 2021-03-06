using System;

namespace SlackNet.Handlers
{
    class SlackServiceFactoryBase : ISlackServiceFactory
    {
        private readonly Lazy<IHttp> _http;
        private readonly Lazy<SlackJsonSettings> _jsonSettings;
        private readonly Lazy<ISlackTypeResolver> _slackTypeResolver;
        private readonly Lazy<ISlackUrlBuilder> _urlBuilder;
        private readonly Lazy<IWebSocketFactory> _webSocketFactory;
        private readonly Lazy<ISlackRequestContextFactory> _requestContextFactory;
        private readonly Lazy<ISlackRequestListener> _requestListener;
        private readonly Lazy<ISlackHandlerFactory> _handlerFactory;
        private readonly Lazy<ISlackApiClient> _apiClient;
        private readonly Lazy<ISlackSocketModeClient> _socketModeClient;

        public SlackServiceFactoryBase(
            Func<ISlackServiceFactory, IHttp> httpProvider,
            Func<ISlackServiceFactory, SlackJsonSettings> jsonSettingsProvider,
            Func<ISlackServiceFactory, ISlackTypeResolver> slackTypeResolverProvider,
            Func<ISlackServiceFactory, ISlackUrlBuilder> urlBuilderProvider,
            Func<ISlackServiceFactory, IWebSocketFactory> webSocketFactoryProvider,
            Func<ISlackServiceFactory, ISlackRequestContextFactory> requestContextFactoryProvider,
            Func<ISlackServiceFactory, ISlackRequestListener> requestListenerProvider,
            Func<ISlackServiceFactory, ISlackHandlerFactory> handlerFactoryProvider,
            Func<ISlackServiceFactory, ISlackApiClient> apiClientProvider,
            Func<ISlackServiceFactory, ISlackSocketModeClient> socketModeClientProvider,
            ISlackServiceFactory dependencyFactory = null)
        {
            dependencyFactory ??= this;
            _http = new Lazy<IHttp>(() => httpProvider(dependencyFactory));
            _jsonSettings = new Lazy<SlackJsonSettings>(() => jsonSettingsProvider(dependencyFactory));
            _slackTypeResolver = new Lazy<ISlackTypeResolver>(() => slackTypeResolverProvider(dependencyFactory));
            _urlBuilder = new Lazy<ISlackUrlBuilder>(() => urlBuilderProvider(dependencyFactory));
            _webSocketFactory = new Lazy<IWebSocketFactory>(() => webSocketFactoryProvider(dependencyFactory));
            _requestContextFactory = new Lazy<ISlackRequestContextFactory>(() => requestContextFactoryProvider(dependencyFactory));
            _requestListener = new Lazy<ISlackRequestListener>(() => requestListenerProvider(dependencyFactory));
            _handlerFactory = new Lazy<ISlackHandlerFactory>(() => handlerFactoryProvider(dependencyFactory));
            _apiClient = new Lazy<ISlackApiClient>(() => apiClientProvider(dependencyFactory));
            _socketModeClient = new Lazy<ISlackSocketModeClient>(() => socketModeClientProvider(dependencyFactory));
        }

        public IHttp GetHttp() => _http.Value;
        public SlackJsonSettings GetJsonSettings() => _jsonSettings.Value;
        public ISlackTypeResolver GetTypeResolver() => _slackTypeResolver.Value;
        public ISlackUrlBuilder GetUrlBuilder() => _urlBuilder.Value;
        public IWebSocketFactory GetWebSocketFactory() => _webSocketFactory.Value;
        public ISlackRequestContextFactory GetRequestContextFactory() => _requestContextFactory.Value;
        public ISlackRequestListener GetRequestListener() => _requestListener.Value;
        public ISlackHandlerFactory GetHandlerFactory() => _handlerFactory.Value;
        public ISlackApiClient GetApiClient() => _apiClient.Value;
        public ISlackSocketModeClient GetSocketModeClient() => _socketModeClient.Value;
    }

    public class SlackServiceCreationException : Exception
    {
        public SlackServiceCreationException(string message) : base(message) { }
    }
}