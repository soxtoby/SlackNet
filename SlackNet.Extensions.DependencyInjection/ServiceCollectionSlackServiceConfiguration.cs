using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    public class ServiceCollectionSlackServiceConfiguration : FactorySlackServiceConfigurationWithDependencyResolver<ServiceCollectionSlackServiceConfiguration, IServiceProvider>
    {
        private readonly IServiceCollection _serviceCollection;
        private ServiceCollectionSlackServiceConfiguration(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

        internal static void Configure(IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
        {
            var config = new ServiceCollectionSlackServiceConfiguration(serviceCollection);
            config.UseRequestListener<ServiceProviderSlackRequestListener>();
            configure?.Invoke(config);

            serviceCollection.AddScoped<HandlerDisposer>();

            serviceCollection.TryAddSingleton<ISlackServiceProvider>(sp => new ServiceProviderSlackServiceProvider(config.CreateServiceFactory, sp));
            serviceCollection.TryAddSingleton<IHttp>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetHttp());
            serviceCollection.TryAddSingleton<SlackJsonSettings>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetJsonSettings());
            serviceCollection.TryAddSingleton<ISlackTypeResolver>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetTypeResolver());
            serviceCollection.TryAddSingleton<ISlackUrlBuilder>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetUrlBuilder());
            serviceCollection.TryAddSingleton<IWebSocketFactory>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetWebSocketFactory());
            serviceCollection.TryAddSingleton<ISlackRequestContextFactory>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetRequestContextFactory());
            serviceCollection.TryAddSingleton<ISlackRequestListener>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetRequestListener());
            serviceCollection.TryAddSingleton<ISlackHandlerFactory>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetHandlerFactory());
            serviceCollection.TryAddSingleton<ISlackApiClient>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetApiClient());
            serviceCollection.TryAddSingleton<ISlackSocketModeClient>(sp => sp.GetRequiredService<ISlackServiceProvider>().GetSocketModeClient());
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService, TImplementation>()
        {
            _serviceCollection.TryAddSingleton<TImplementation>();
            return serviceFactory => ((ServiceProviderSlackServiceProvider)serviceFactory).GetRequiredService<TImplementation>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>()
        {
            _serviceCollection.TryAddScoped<TImplementation>();
            return requestContext => requestContext.ServiceScope().ServiceProvider.GetRequiredService<TImplementation>();
        }

        protected override Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>()
        {
            _serviceCollection.TryAddScoped<THandler>();
            return requestContext => requestContext.ServiceScope().ServiceProvider.GetRequiredService<THandler>();
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService>(Func<IServiceProvider, TService> getService)
        {
            _serviceCollection.AddSingleton(getService);
            return serviceFactory => ((ServiceProviderSlackServiceProvider)serviceFactory).GetRequiredService<TService>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<IServiceProvider, THandler> getHandler) =>
            requestContext =>
                {
                    var handler = getHandler(requestContext.ServiceScope().ServiceProvider);
                    if (handler is IDisposable disposable)
                        requestContext.ServiceScope().ServiceProvider.GetRequiredService<HandlerDisposer>().Add(disposable);
                    return handler;
                };
    }
}
