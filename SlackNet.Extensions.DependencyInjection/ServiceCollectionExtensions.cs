using System;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
    {
        var config = new ServiceCollectionSlackServiceConfiguration(serviceCollection);
        
        configure?.Invoke(config);
        
        config.ConfigureServices();
        
        return serviceCollection;
    }
}