using System;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
    {
        ServiceCollectionSlackServiceConfiguration.Configure(serviceCollection, configure);
        return serviceCollection;
    }
}