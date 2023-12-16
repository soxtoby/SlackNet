using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection;

public class ServiceCollectionSlackServiceConfiguration : ServiceCollectionSlackServiceConfiguration<ServiceCollectionSlackServiceConfiguration>
{
    internal ServiceCollectionSlackServiceConfiguration(IServiceCollection serviceCollection)
        : base(serviceCollection) { }
}

[SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
public abstract class ServiceCollectionSlackServiceConfiguration<TConfig> : FactorySlackServiceConfigurationWithDependencyResolver<TConfig, IServiceProvider> 
    where TConfig : ServiceCollectionSlackServiceConfiguration<TConfig>
{
    protected internal ServiceCollectionSlackServiceConfiguration(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
        UseRequestListener<IServiceProviderSlackRequestListener>();
    }

    protected IServiceCollection ServiceCollection { get; }

    protected internal void ConfigureServices()
    {
        ServiceCollection.TryAddSingleton<IServiceProviderSlackRequestListener, ServiceProviderSlackRequestListener>();

        ServiceCollection.AddScoped<HandlerDisposer>();

        ServiceCollection.TryAddSingleton<ISlackServiceProvider>(sp => new ServiceProviderSlackServiceProvider(CreateServiceFactory, sp));
        ServiceCollection.TryAddSingleton<IHttp>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetHttp());
        ServiceCollection.TryAddSingleton<SlackJsonSettings>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetJsonSettings());
        ServiceCollection.TryAddSingleton<ISlackTypeResolver>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetTypeResolver());
        ServiceCollection.TryAddSingleton<ISlackUrlBuilder>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetUrlBuilder());
        ServiceCollection.TryAddSingleton<ILogger>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetLogger());
        ServiceCollection.TryAddSingleton<IWebSocketFactory>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetWebSocketFactory());
        ServiceCollection.TryAddSingleton<IEnumerable<ISlackRequestListener>>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetRequestListeners());
        ServiceCollection.TryAddSingleton<ISlackHandlerFactory>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetHandlerFactory());
        ServiceCollection.TryAddSingleton<ISlackApiClient>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetApiClient());
        ServiceCollection.TryAddSingleton<ISlackSocketModeClient>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetSocketModeClient());
    }

    protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService, TImplementation>()
    {
        if (ShouldRegisterType<TImplementation>())
            ServiceCollection.TryAddSingleton<TImplementation>();
        return serviceFactory => ((ServiceProviderSlackServiceProvider) serviceFactory).GetRequiredService<TImplementation>();
    }

    protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>()
    {
        if (ShouldRegisterType<TImplementation>())
            ServiceCollection.TryAddScoped<TImplementation>();
        return requestContext => requestContext.ServiceProvider().GetRequiredService<TImplementation>();
    }

    protected override Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>()
    {
        if (ShouldRegisterType<THandler>())
            ServiceCollection.TryAddScoped<THandler>();
        return requestContext => requestContext.ServiceProvider().GetRequiredService<THandler>();
    }

    protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService>(Func<IServiceProvider, TService> getService)
    {
        ServiceCollection.AddSingleton(getService);
        return serviceFactory => ((ServiceProviderSlackServiceProvider) serviceFactory).GetRequiredService<TService>();
    }

    protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<IServiceProvider, THandler> getHandler) =>
        requestContext =>
            {
                var handler = getHandler(requestContext.ServiceProvider());
                if (handler is IDisposable disposable)
                    requestContext.ServiceProvider().GetRequiredService<HandlerDisposer>().Add(disposable);
                return handler;
            };
}