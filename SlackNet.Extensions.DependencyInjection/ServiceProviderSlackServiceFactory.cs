using System;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    class ServiceProviderSlackServiceFactory : ISlackServiceFactory
    {
        private readonly ISlackServiceFactory _baseFactory;
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderSlackServiceFactory(Func<ISlackServiceFactory, ISlackServiceFactory> createBaseFactory, IServiceProvider serviceProvider)
        {
            _baseFactory = createBaseFactory(this);
            _serviceProvider = serviceProvider;
        }

        public TService GetRequiredService<TService>() where TService : class => _serviceProvider.GetRequiredService<TService>();

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