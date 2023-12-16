using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SlackNet.AzureFunctions;

public static class ApplicationBuilderExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection services, Action<FunctionsSlackServiceConfiguration>? configure = null)
    {
        var config = new FunctionsSlackServiceConfiguration(services);
        configure?.Invoke(config);
        config.ConfigureServices();
        return services;
    }
    
    public static IFunctionsWorkerApplicationBuilder UseSlackNet(this IFunctionsWorkerApplicationBuilder appBuilder, Action<FunctionsSlackServiceConfiguration>? configure = null) => 
        appBuilder.UseMiddleware<FunctionContextServiceProviderMiddleware>();
}