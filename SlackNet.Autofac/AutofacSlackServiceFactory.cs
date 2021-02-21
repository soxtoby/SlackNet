using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    class AutofacSlackServiceFactory : ISlackServiceFactory
    {
        private readonly ILifetimeScope _scope;
        public AutofacSlackServiceFactory(ILifetimeScope scope) => _scope = scope;

        public IHttp GetHttp() => _scope.Resolve<IHttp>();
        public SlackJsonSettings GetJsonSettings() => _scope.Resolve<SlackJsonSettings>();
        public ISlackTypeResolver GetTypeResolver() => _scope.Resolve<ISlackTypeResolver>();
        public ISlackUrlBuilder GetUrlBuilder() => _scope.Resolve<ISlackUrlBuilder>();
        public IWebSocketFactory GetWebSocketFactory() => _scope.Resolve<IWebSocketFactory>();
        public ISlackRequestListener GetRequestListener() => _scope.Resolve<ISlackRequestListener>();
        public ISlackHandlerFactory GetHandlerFactory() => _scope.Resolve<ISlackHandlerFactory>();
        public ISlackApiClient GetApiClient() => _scope.Resolve<ISlackApiClient>();
        public ISlackSocketModeClient GetSocketModeClient() => _scope.Resolve<ISlackSocketModeClient>();
    }
}