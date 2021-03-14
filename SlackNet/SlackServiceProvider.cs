using System;
using SlackNet.Handlers;

namespace SlackNet
{
    public interface ISlackServiceProvider
    {
        IHttp GetHttp();
        SlackJsonSettings GetJsonSettings();
        ISlackTypeResolver GetTypeResolver();
        ISlackUrlBuilder GetUrlBuilder();
        IWebSocketFactory GetWebSocketFactory();
        ISlackRequestContextFactory GetRequestContextFactory();
        ISlackRequestListener GetRequestListener();
        ISlackHandlerFactory GetHandlerFactory();
        ISlackApiClient GetApiClient();
        ISlackSocketModeClient GetSocketModeClient();
    }

    class SlackServiceProvider : ISlackServiceProvider
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

        public SlackServiceProvider(
            Func<ISlackServiceProvider, IHttp> httpProvider,
            Func<ISlackServiceProvider, SlackJsonSettings> jsonSettingsProvider,
            Func<ISlackServiceProvider, ISlackTypeResolver> slackTypeResolverProvider,
            Func<ISlackServiceProvider, ISlackUrlBuilder> urlBuilderProvider,
            Func<ISlackServiceProvider, IWebSocketFactory> webSocketFactoryProvider,
            Func<ISlackServiceProvider, ISlackRequestContextFactory> requestContextFactoryProvider,
            Func<ISlackServiceProvider, ISlackRequestListener> requestListenerProvider,
            Func<ISlackServiceProvider, ISlackHandlerFactory> handlerFactoryProvider,
            Func<ISlackServiceProvider, ISlackApiClient> apiClientProvider,
            Func<ISlackServiceProvider, ISlackSocketModeClient> socketModeClientProvider,
            ISlackServiceProvider dependencyProvider = null)
        {
            dependencyProvider ??= this;
            _http = new Lazy<IHttp>(() => httpProvider(dependencyProvider));
            _jsonSettings = new Lazy<SlackJsonSettings>(() => jsonSettingsProvider(dependencyProvider));
            _slackTypeResolver = new Lazy<ISlackTypeResolver>(() => slackTypeResolverProvider(dependencyProvider));
            _urlBuilder = new Lazy<ISlackUrlBuilder>(() => urlBuilderProvider(dependencyProvider));
            _webSocketFactory = new Lazy<IWebSocketFactory>(() => webSocketFactoryProvider(dependencyProvider));
            _requestContextFactory = new Lazy<ISlackRequestContextFactory>(() => requestContextFactoryProvider(dependencyProvider));
            _requestListener = new Lazy<ISlackRequestListener>(() => requestListenerProvider(dependencyProvider));
            _handlerFactory = new Lazy<ISlackHandlerFactory>(() => handlerFactoryProvider(dependencyProvider));
            _apiClient = new Lazy<ISlackApiClient>(() => apiClientProvider(dependencyProvider));
            _socketModeClient = new Lazy<ISlackSocketModeClient>(() => socketModeClientProvider(dependencyProvider));
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