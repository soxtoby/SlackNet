using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Handlers;

namespace SlackNet.Extensions.DependencyInjection
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    public class ServiceCollectionSlackHandlerConfiguration : FactorySlackHandlerConfigurationWithDependencyResolver<ServiceCollectionSlackHandlerConfiguration, IServiceProvider>
    {
        private readonly IServiceCollection _serviceCollection;
        private ServiceCollectionSlackHandlerConfiguration(IServiceCollection serviceCollection) => _serviceCollection = serviceCollection;

        internal static void Configure(IServiceCollection serviceCollection, Action<ServiceCollectionSlackHandlerConfiguration> configure = null)
        {
            var config = new ServiceCollectionSlackHandlerConfiguration(serviceCollection);
            config.UseRequestListener<ServiceProviderSlackRequestListener>();
            configure?.Invoke(config);

            serviceCollection.AddScoped<HandlerDisposer>();

            serviceCollection.TryAddSingleton<ISlackServiceFactory>(sp => new ServiceProviderSlackServiceFactory(config.CreateServiceFactory, sp));
            serviceCollection.TryAddSingleton<IHttp>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetHttp());
            serviceCollection.TryAddSingleton<SlackJsonSettings>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetJsonSettings());
            serviceCollection.TryAddSingleton<ISlackTypeResolver>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetTypeResolver());
            serviceCollection.TryAddSingleton<ISlackUrlBuilder>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetUrlBuilder());
            serviceCollection.TryAddSingleton<IWebSocketFactory>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetWebSocketFactory());
            serviceCollection.TryAddSingleton<ISlackRequestListener>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetRequestListener());
            serviceCollection.TryAddSingleton<ISlackHandlerFactory>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetHandlerFactory());
            serviceCollection.TryAddSingleton<ISlackApiClient>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetApiClient());
            serviceCollection.TryAddSingleton<ISlackSocketModeClient>(sp => sp.GetRequiredService<ISlackServiceFactory>().GetSocketModeClient());
        }

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService, TImplementation>()
        {
            _serviceCollection.TryAddSingleton<TImplementation>();
            return serviceFactory => ((ServiceProviderSlackServiceFactory)serviceFactory).GetRequiredService<TImplementation>();
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

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService>(Func<IServiceProvider, TService> getService)
        {
            _serviceCollection.AddSingleton(getService);
            return serviceFactory => ((ServiceProviderSlackServiceFactory)serviceFactory).GetRequiredService<TService>();
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