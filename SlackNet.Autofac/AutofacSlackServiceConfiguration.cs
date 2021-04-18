using System;
using System.Collections.Generic;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    public class AutofacSlackServiceConfiguration : FactorySlackServiceConfigurationWithDependencyResolver<AutofacSlackServiceConfiguration, IComponentContext>
    {
        private readonly ContainerBuilder _containerBuilder;
        private AutofacSlackServiceConfiguration(ContainerBuilder containerBuilder) => _containerBuilder = containerBuilder;

        internal static void Configure(ContainerBuilder containerBuilder, Action<AutofacSlackServiceConfiguration> configure = null)
        {
            var config = new AutofacSlackServiceConfiguration(containerBuilder);

            containerBuilder.Register(c => new AutofacSlackServiceProvider(config.CreateServiceFactory, c.Resolve<ILifetimeScope>()))
                .As<ISlackServiceProvider>()
                .SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetHttp()).As<IHttp>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetJsonSettings()).As<SlackJsonSettings>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetTypeResolver()).As<ISlackTypeResolver>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetUrlBuilder()).As<ISlackUrlBuilder>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetWebSocketFactory()).As<IWebSocketFactory>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetRequestListeners()).As<IEnumerable<ISlackRequestListener>>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetHandlerFactory()).As<ISlackHandlerFactory>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetApiClient()).As<ISlackApiClient>().SingleInstance();
            containerBuilder.Register(c => c.Resolve<ISlackServiceProvider>().GetSocketModeClient()).As<ISlackSocketModeClient>().SingleInstance();

            config.UseRequestListener<AutofacSlackRequestListener>();
            configure?.Invoke(config);
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService, TImplementation>()
        {
            if (ShouldRegisterType<TImplementation>())
                _containerBuilder.RegisterType<TImplementation>().As<TService>().SingleInstance();
            return serviceFactory => ((AutofacSlackServiceProvider)serviceFactory).Resolve<TService>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler, TImplementation>()
        {
            if (ShouldRegisterType<TImplementation>())
                _containerBuilder.RegisterType<TImplementation>().As<THandler>().InstancePerLifetimeScope();
            return requestContext => requestContext.LifetimeScope().Resolve<THandler>();
        }

        protected override Func<SlackRequestContext, THandler> GetRegisteredHandlerFactory<THandler>()
        {
            if (ShouldRegisterType<THandler>())
                _containerBuilder.RegisterType<THandler>().InstancePerLifetimeScope();
            return requestContext => requestContext.LifetimeScope().Resolve<THandler>();
        }

        protected override Func<ISlackServiceProvider, TService> GetServiceFactory<TService>(Func<IComponentContext, TService> getService)
        {
            _containerBuilder.Register(getService).SingleInstance();
            return serviceFactory => ((AutofacSlackServiceProvider)serviceFactory).Resolve<TService>();
        }

        protected override Func<SlackRequestContext, THandler> GetRequestHandlerFactory<THandler>(Func<IComponentContext, THandler> getHandler) =>
            requestContext => getHandler(requestContext.LifetimeScope());
    }
}