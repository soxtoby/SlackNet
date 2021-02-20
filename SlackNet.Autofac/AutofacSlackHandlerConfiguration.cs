using System;
using Autofac;
using SlackNet.Handlers;

namespace SlackNet.Autofac
{
    public class AutofacSlackHandlerConfiguration : FactorySlackHandlerConfigurationWithDependencyResolver<AutofacSlackHandlerConfiguration, IComponentContext>
    {
        private readonly ContainerBuilder _containerBuilder;
        public AutofacSlackHandlerConfiguration(ContainerBuilder containerBuilder) => _containerBuilder = containerBuilder;

        protected override void ReplaceClientService<TService>(Func<IComponentContext, TService> serviceFactory) =>
            _containerBuilder.Register(serviceFactory).SingleInstance();

        protected override void ReplaceClientService<TService, TImplementation>() =>
            _containerBuilder.RegisterType<TImplementation>().As<TService>().SingleInstance();

        protected override void ReplaceCollectionHandling<THandler>(CollectionHandlerFactory<IComponentContext, THandler> handlerFactory) =>
            _containerBuilder.RegisterComposite<THandler>((c, hs) => handlerFactory(c, hs)).InstancePerLifetimeScope();

        protected override void ReplaceKeyedHandling<THandler>(KeyedHandlerFactory<IComponentContext, THandler> handlerFactory) =>
            _containerBuilder.Register(c => handlerFactory(c, c.Resolve<IHandlerIndex<THandler>>())).InstancePerLifetimeScope();

        protected override void ReplaceCollectionHandling<THandler, TImplementation>() =>
            _containerBuilder.RegisterComposite<TImplementation, THandler>().InstancePerLifetimeScope();

        protected override void ReplaceKeyedHandler<THandler, TImplementation>() =>
            _containerBuilder.RegisterType<TImplementation>().As<THandler>().InstancePerLifetimeScope();

        protected override void AddCollectionHandler<THandler>(THandler handler) =>
            _containerBuilder.RegisterInstance(handler);

        protected override void AddKeyedHandler<THandler>(string key, THandler handler) =>
            _containerBuilder.RegisterInstance(handler).Named<THandler>(key);

        protected override void AddCollectionHandler<TInnerHandler, TOuterHandler>(Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _containerBuilder.RegisterType<TInnerHandler>().InstancePerLifetimeScope();
            _containerBuilder.Register(c => adaptor(c.Resolve<TInnerHandler>())).InstancePerLifetimeScope();
        }

        protected override void AddKeyedHandler<TInnerHandler, TOuterHandler>(string key, Func<TInnerHandler, TOuterHandler> adaptor)
        {
            _containerBuilder.RegisterType<TInnerHandler>().InstancePerLifetimeScope();
            _containerBuilder.Register(c => adaptor(c.Resolve<TInnerHandler>())).Named<TOuterHandler>(key);
        }

        protected override void AddCollectionHandler<THandler>(Func<IComponentContext, THandler> handlerFactory)
        {
            _containerBuilder.Register(handlerFactory).InstancePerLifetimeScope();
        }

        protected override void AddKeyedHandler<THandler>(string key, Func<IComponentContext, THandler> handlerFactory)
        {
            _containerBuilder.Register(handlerFactory).Named<THandler>(key).InstancePerLifetimeScope();
        }
    }
}