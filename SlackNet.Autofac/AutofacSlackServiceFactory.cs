using System;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    class AutofacSlackServiceFactory : ISlackServiceFactory
    {
        private readonly ISlackServiceFactory _baseFactory;
        private readonly ILifetimeScope _scope;

        public AutofacSlackServiceFactory(Func<ISlackServiceFactory, ISlackServiceFactory> createBaseFactory, ILifetimeScope scope)
        {
            _baseFactory = createBaseFactory(this);
            _scope = scope;
        }

        public T Resolve<T>() => _scope.Resolve<T>();

        public T Resolve<T>(Func<IComponentContext, T> getService)
        {
            var service = getService(_scope);
            if (service is IDisposable disposable)
                _scope.Disposer.AddInstanceForDisposal(disposable);
            else if (service is IAsyncDisposable asyncDisposable)
                _scope.Disposer.AddInstanceForAsyncDisposal(asyncDisposable);
            return service;
        }

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