using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.AspNetCore;

namespace SlackNet.AzureFunctions;

public class FunctionsSlackServiceConfiguration : FunctionsSlackServiceConfiguration<FunctionsSlackServiceConfiguration>
{
    internal FunctionsSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection) { }
}

public abstract class FunctionsSlackServiceConfiguration<TConfig>: AspNetSlackServiceConfiguration<TConfig> 
    where TConfig : FunctionsSlackServiceConfiguration<TConfig>
{
    private bool _inProcessFunction = Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME") == "dotnet";
    
    internal FunctionsSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection) { }

    internal new void ConfigureServices()
    {
        if (!_inProcessFunction) // In-process will use the regular ASP.NET service provider accessor
        {
            ServiceCollection.TryAddSingleton<FunctionContextServiceProviderAccessor>();
            ServiceCollection.TryAddSingleton<IRequestServiceProviderAccessor, FunctionContextServiceProviderAccessor>();
        }
        
        ServiceCollection.TryAddSingleton<ISlackFunctionRequestHandler, SlackFunctionRequestHandler>();

        base.ConfigureServices();
    }
    
    /// <summary>
    /// In-process functions require different services to isolated worker functions.
    /// If the default in-process function detection doesn't work for you, you can override it with this.
    /// </summary>
    public TConfig InProcessFunction(bool inProcess = true) => Chain(() => _inProcessFunction = inProcess);
}