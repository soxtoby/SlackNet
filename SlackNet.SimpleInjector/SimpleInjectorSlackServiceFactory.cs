using System;
using SimpleInjector;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    class SimpleInjectorSlackServiceFactory : ISlackServiceFactory
    {
        private readonly ISlackServiceFactory _baseFactory;
        private readonly Container _container;

        public SimpleInjectorSlackServiceFactory(Func<ISlackServiceFactory, ISlackServiceFactory> createBaseFactory, Container container)
        {
            _baseFactory = createBaseFactory(this);
            _container = container;
        }

        public TService GetInstance<TService>() where TService : class => _container.GetInstance<TService>();

        public IHttp GetHttp() => _baseFactory.GetHttp();
        public SlackJsonSettings GetJsonSettings() => _baseFactory.GetJsonSettings();
        public ISlackTypeResolver GetTypeResolver() => _baseFactory.GetTypeResolver();
        public ISlackUrlBuilder GetUrlBuilder() => _baseFactory.GetUrlBuilder();
        public IWebSocketFactory GetWebSocketFactory() => _baseFactory.GetWebSocketFactory();
        public ISlackRequestContextFactory GetRequestContextFactory() => _baseFactory.GetRequestContextFactory();
        public ISlackRequestListener GetRequestListener() => _baseFactory.GetRequestListener();
        public ISlackHandlerFactory GetHandlerFactory() => _baseFactory.GetHandlerFactory();
        public ISlackApiClient GetApiClient() => _baseFactory.GetApiClient();
        public ISlackSocketModeClient GetSocketModeClient() => _baseFactory.GetSocketModeClient();
    }
}