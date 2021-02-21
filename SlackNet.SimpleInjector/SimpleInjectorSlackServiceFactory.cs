using SimpleInjector;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    class SimpleInjectorSlackServiceFactory : ISlackServiceFactory
    {
        private readonly Container _container;
        public SimpleInjectorSlackServiceFactory(Container container) => _container = container;

        public IHttp GetHttp() => _container.GetInstance<IHttp>();
        public SlackJsonSettings GetJsonSettings() => _container.GetInstance<SlackJsonSettings>();
        public ISlackTypeResolver GetTypeResolver() => _container.GetInstance<ISlackTypeResolver>();
        public ISlackUrlBuilder GetUrlBuilder() => _container.GetInstance<ISlackUrlBuilder>();
        public IWebSocketFactory GetWebSocketFactory() => _container.GetInstance<IWebSocketFactory>();
        public ISlackRequestListener GetRequestListener() => _container.GetInstance<ISlackRequestListener>();
        public ISlackHandlerFactory GetHandlerFactory() => _container.GetInstance<ISlackHandlerFactory>();
        public ISlackApiClient GetApiClient() => _container.GetInstance<ISlackApiClient>();
        public ISlackSocketModeClient GetSocketModeClient() => _container.GetInstance<ISlackSocketModeClient>();
    }
}