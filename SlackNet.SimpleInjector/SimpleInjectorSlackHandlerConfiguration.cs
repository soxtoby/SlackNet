using System;
using System.Diagnostics.CodeAnalysis;
using SimpleInjector;
using SlackNet.Handlers;

namespace SlackNet.SimpleInjector
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    public class SimpleInjectorSlackHandlerConfiguration : FactorySlackHandlerConfigurationWithExternalDependencyResolver<SimpleInjectorSlackHandlerConfiguration>
    {
        private readonly Container _container;
        private SimpleInjectorSlackHandlerConfiguration(Container container) => _container = container;

        internal static void Configure(Container container, Action<SimpleInjectorSlackHandlerConfiguration> configure = null)
        {
            var config = new SimpleInjectorSlackHandlerConfiguration(container);
            config.UseRequestListener<SimpleInjectorSlackRequestListener>();
            configure?.Invoke(config);

            RegisterFallback<ISlackServiceFactory>(container, () => new SimpleInjectorSlackServiceFactory(config.CreateServiceFactory, container), Lifestyle.Singleton);
            RegisterFallback<IHttp>(container, () => container.GetInstance<ISlackServiceFactory>().GetHttp(), Lifestyle.Singleton);
            RegisterFallback<SlackJsonSettings>(container, () => container.GetInstance<ISlackServiceFactory>().GetJsonSettings(), Lifestyle.Singleton);
            RegisterFallback<ISlackTypeResolver>(container, () => container.GetInstance<ISlackServiceFactory>().GetTypeResolver(), Lifestyle.Singleton);
            RegisterFallback<ISlackUrlBuilder>(container, () => container.GetInstance<ISlackServiceFactory>().GetUrlBuilder(), Lifestyle.Singleton);
            RegisterFallback<IWebSocketFactory>(container, () => container.GetInstance<ISlackServiceFactory>().GetWebSocketFactory(), Lifestyle.Singleton);
            RegisterFallback<ISlackRequestListener>(container, () => container.GetInstance<ISlackServiceFactory>().GetRequestListener(), Lifestyle.Singleton);
            RegisterFallback<ISlackHandlerFactory>(container, () => container.GetInstance<ISlackServiceFactory>().GetHandlerFactory(), Lifestyle.Singleton);
            RegisterFallback<ISlackApiClient>(container, () => container.GetInstance<ISlackServiceFactory>().GetApiClient(), Lifestyle.Singleton);
            RegisterFallback<ISlackSocketModeClient>(container, () => container.GetInstance<ISlackServiceFactory>().GetSocketModeClient(), Lifestyle.Singleton);
        }

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService, TImplementation>()
        {
            RegisterFallbackType<TImplementation>(Lifestyle.Singleton);
            return serviceFactory => ((SimpleInjectorSlackServiceFactory)serviceFactory).GetInstance<TImplementation>();
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

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService>(Func<TService> getService)
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