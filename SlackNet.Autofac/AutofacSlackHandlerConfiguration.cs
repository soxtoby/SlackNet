using System;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    public class AutofacSlackHandlerConfiguration : FactorySlackHandlerConfigurationWithDependencyResolver<AutofacSlackHandlerConfiguration, IComponentContext>
    {
        private readonly ContainerBuilder _containerBuilder;
        private AutofacSlackHandlerConfiguration(ContainerBuilder containerBuilder) => _containerBuilder = containerBuilder;

        internal static void Configure(ContainerBuilder containerBuilder, Action<AutofacSlackHandlerConfiguration> configure = null)
        {
            var config = new AutofacSlackHandlerConfiguration(containerBuilder);

            containerBuilder.Register(c => new AutofacSlackServiceFactory(config.CreateServiceFactory, c.Resolve<ILifetimeScope>()))
                .As<ISlackServiceFactory>()
                .SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetHttp()).As<IHttp>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetJsonSettings()).As<SlackJsonSettings>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetTypeResolver()).As<ISlackTypeResolver>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetUrlBuilder()).As<ISlackUrlBuilder>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetWebSocketFactory()).As<IWebSocketFactory>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetRequestListener()).As<ISlackRequestListener>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetHandlerFactory()).As<ISlackHandlerFactory>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetApiClient()).As<ISlackApiClient>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceFactory>().GetSocketModeClient()).As<ISlackSocketModeClient>().SingleInstance();

            config.UseRequestListener<AutofacSlackRequestListener>();
            configure?.Invoke(config);
        }

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService, TImplementation>()
        {
            _containerBuilder.RegisterType<TImplementation>().As<TService>().SingleInstance();
            return serviceFactory => ((AutofacSlackServiceFactory)serviceFactory).Resolve<TService>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>()
        {
            _containerBuilder.RegisterType<TImplementation>().As<THandler>().InstancePerLifetimeScope();
            return requestContext => requestContext.LifetimeScope().Resolve<THandler>();
        }

        protected override Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>()
        {
            _containerBuilder.RegisterType<THandler>().InstancePerLifetimeScope();
            return requestContext => requestContext.LifetimeScope().Resolve<THandler>();
        }

        protected override Func<ISlackServiceFactory, TService> GetServiceFactory<TService>(Func<IComponentContext, TService> getService) =>
            serviceFactory => ((AutofacSlackServiceFactory)serviceFactory).Resolve(getService);

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<IComponentContext, THandler> getHandler) =>
            requestContext => getHandler(requestContext.LifetimeScope());
    }
}