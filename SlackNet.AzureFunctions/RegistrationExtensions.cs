using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Extensions.DependencyInjection;
using ServiceCollectionExtensions = SlackNet.Extensions.DependencyInjection.ServiceCollectionExtensions;

namespace SlackNet.AzureFunctions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
    {
        serviceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
        //serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //serviceCollection.TryAddSingleton<IServiceProviderSlackRequestListener, AspNetCoreServiceProviderSlackRequestListener>();
        return ServiceCollectionExtensions.AddSlackNet(serviceCollection, c =>
            {
                c.UseLogger<MicrosoftLoggerAdaptor>();
                configure?.Invoke(c);
            });
    }
}