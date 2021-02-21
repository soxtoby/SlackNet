using System;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    class ServiceProviderSlackServiceFactory : ISlackServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ServiceProviderSlackServiceFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IHttp GetHttp() => _serviceProvider.GetRequiredService<IHttp>();
        public SlackJsonSettings GetJsonSettings() => _serviceProvider.GetRequiredService<SlackJsonSettings>();
        public ISlackTypeResolver GetTypeResolver() => _serviceProvider.GetRequiredService<ISlackTypeResolver>();
        public ISlackUrlBuilder GetUrlBuilder() => _serviceProvider.GetRequiredService<ISlackUrlBuilder>();
        public IWebSocketFactory GetWebSocketFactory() => _serviceProvider.GetRequiredService<IWebSocketFactory>();
        public ISlackRequestListener GetRequestListener() => _serviceProvider.GetRequiredService<ISlackRequestListener>();
        public ISlackHandlerFactory GetHandlerFactory() => _serviceProvider.GetRequiredService<ISlackHandlerFactory>();
        public ISlackApiClient GetApiClient() => _serviceProvider.GetRequiredService<ISlackApiClient>();
        public ISlackSocketModeClient GetSocketModeClient() => _serviceProvider.GetRequiredService<ISlackSocketModeClient>();
    }
}