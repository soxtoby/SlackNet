using System;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    class ServiceProviderSlackServiceProvider : ISlackServiceProvider
    {
        private readonly ISlackServiceProvider _baseProvider;
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderSlackServiceProvider(Func<ISlackServiceProvider, ISlackServiceProvider> createBaseProvider, IServiceProvider serviceProvider)
        {
            _baseProvider = createBaseProvider(this);
            _serviceProvider = serviceProvider;
        }

        public TService GetRequiredService<TService>() where TService : class => _serviceProvider.GetRequiredService<TService>();

        public IHttp GetHttp() => _baseProvider.GetHttp();
        public SlackJsonSettings GetJsonSettings() => _baseProvider.GetJsonSettings();
        public ISlackTypeResolver GetTypeResolver() => _baseProvider.GetTypeResolver();
        public ISlackUrlBuilder GetUrlBuilder() => _baseProvider.GetUrlBuilder();
        public IWebSocketFactory GetWebSocketFactory() => _baseProvider.GetWebSocketFactory();
        public ISlackRequestContextFactory GetRequestContextFactory() => _baseProvider.GetRequestContextFactory();
        public ISlackRequestListener GetRequestListener() => _baseProvider.GetRequestListener();
        public ISlackHandlerFactory GetHandlerFactory() => _baseProvider.GetHandlerFactory();
        public ISlackApiClient GetApiClient() => _baseProvider.GetApiClient();
        public ISlackSocketModeClient GetSocketModeClient() => _baseProvider.GetSocketModeClient();
    }
}