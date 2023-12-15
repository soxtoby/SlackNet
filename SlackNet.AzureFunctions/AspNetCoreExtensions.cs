using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.AspNetCore;
using SlackNet.Extensions.DependencyInjection;
using ServiceCollectionExtensions = SlackNet.Extensions.DependencyInjection.ServiceCollectionExtensions;

namespace SlackNet.AzureFunctions;

public static class AzureFunctionExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
    {
        serviceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
        serviceCollection.TryAddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();
        serviceCollection.TryAddSingleton<IServiceProviderSlackRequestListener, IsolatedWorkerServiceProviderSlackRequestListener>();
        return ServiceCollectionExtensions.AddSlackNet(serviceCollection, c =>
            {
                c.UseLogger<MicrosoftLoggerAdaptor>();
                configure?.Invoke(c);
            });
    }
}