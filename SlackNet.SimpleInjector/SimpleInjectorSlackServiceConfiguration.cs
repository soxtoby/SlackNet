using System;
using System.Diagnostics.CodeAnalysis;
using SimpleInjector;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    public class SimpleInjectorSlackServiceConfiguration : FactorySlackServiceConfigurationWithExternalDependencyResolver<SimpleInjectorSlackServiceConfiguration>
    {
        private readonly Container _container;
        private SimpleInjectorSlackServiceConfiguration(Container container) => _container = container;

        internal static void Configure(Container container, Action<SimpleInjectorSlackServiceConfiguration> configure = null)
        {
            var config = new SimpleInjectorSlackServiceConfiguration(container);
            config.UseRequestListener<SimpleInjectorSlackRequestListener>();
            configure?.Invoke(config);

            RegisterFallback<ISlackServiceProvider>(container, () => new SimpleInjectorSlackServiceProvider(config.CreateServiceFactory, container), Lifestyle.Singleton);
            RegisterFallback<IHttp>(container, () => container.GetInstance<ISlackServiceProvider>().GetHttp(), Lifestyle.Singleton);
            RegisterFallback<SlackJsonSettings>(container, () => container.GetInstance<ISlackServiceProvider>().GetJsonSettings(), Lifestyle.Singleton);
            RegisterFallback<ISlackTypeResolver>(container, () => container.GetInstance<ISlackServiceProvider>().GetTypeResolver(), Lifestyle.Singleton);
            RegisterFallback<ISlackUrlBuilder>(container, () => container.GetInstance<ISlackServiceProvider>().GetUrlBuilder(), Lifestyle.Singleton);
            RegisterFallback<IWebSocketFactory>(container, () => container.GetInstance<ISlackServiceProvider>().GetWebSocketFactory(), Lifestyle.Singleton);
            RegisterFallback<ISlackRequestContextFactory>(container, () => container.GetInstance<ISlackServiceProvider>().GetRequestContextFactory(), Lifestyle.Singleton);
            RegisterFallback<ISlackRequestListener>(container, () => container.GetInstance<ISlackServiceProvider>().GetRequestListener(), Lifestyle.Singleton);
            RegisterFallback<ISlackHandlerFactory>(container, () => container.GetInstance<ISlackServiceProvider>().GetHandlerFactory(), Lifestyle.Singleton);
            RegisterFallback<ISlackApiClient>(container, () => container.GetInstance<ISlackServiceProvider>().GetApiClient(), Lifestyle.Singleton);
            RegisterFallback<ISlackSocketModeClient>(container, () => container.GetInstance<ISlackServiceProvider>().GetSocketModeClient(), Lifestyle.Singleton);
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService, TImplementation>()
        {
            RegisterFallbackType<TImplementation>(Lifestyle.Singleton);
            return serviceFactory => ((SimpleInjectorSlackServiceProvider)serviceFactory).GetInstance<TImplementation>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>()
        {
            RegisterFallbackType<TImplementation>(Lifestyle.Scoped);
            return requestContext => requestContext.ContainerScope().GetInstance<TImplementation>();
        }

        protected override Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>()
        {
            RegisterFallbackType<THandler>(Lifestyle.Scoped);
            return requestContext => requestContext.ContainerScope().GetInstance<THandler>();
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService>(Func<TService> getService)
        {
            var instanceProducer = Lifestyle.Singleton.CreateProducer(getService, _container);
            return _ => instanceProducer.GetInstance();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<THandler> getHandler)
        {
            var instanceProducer = Lifestyle.Scoped.CreateProducer(getHandler, _container);
            return _ => instanceProducer.GetInstance();
        }

        private void RegisterFallbackType<TImplementation>(Lifestyle lifestyle) where TImplementation : class =>
            RegisterFallback<TImplementation>(_container, a => a.Register(lifestyle.CreateProducer<TImplementation, TImplementation>(_container).Registration));

        private static void RegisterFallback<TService>(Container container, Func<TService> factory, Lifestyle lifestyle) where TService : class =>
            RegisterFallback<TService>(container, a => a.Register(lifestyle.CreateRegistration(factory, container)));

        private static void RegisterFallback<TService>(Container container, Action<UnregisteredTypeEventArgs> register) =>
            container.ResolveUnregisteredType += (_, args) =>
                {
                    if (!args.Handled && args.UnregisteredServiceType == typeof(TService))
                        register(args);
                };
    }
}
