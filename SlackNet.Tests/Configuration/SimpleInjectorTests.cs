using System;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SlackNet.Handlers;
using SlackNet.SimpleInjector;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class SimpleInjectorTests : FactorySlackHandlerConfigurationWithExternalDependencyResolverTests<SimpleInjectorSlackHandlerConfiguration>
    {
        private Container _container;

        [SetUp]
        public void SetUp()
        {
            _container = DefaultContainer();
        }

        protected override void ReplaceCollectionHandling<THandler>(Action<SimpleInjectorSlackHandlerConfiguration, CollectionHandlerFactory<THandler>> replaceHandling, Action<SimpleInjectorSlackHandlerConfiguration, THandler> registerHandler, Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
        {
            _container.Options.EnableAutoVerification = false;
            base.ReplaceCollectionHandling(replaceHandling, registerHandler, getHandler);
        }

        protected override void ReplaceKeyedHandling<THandler>(Action<SimpleInjectorSlackHandlerConfiguration, KeyedHandlerFactory<THandler>> replaceHandling, Action<SimpleInjectorSlackHandlerConfiguration, string, THandler> registerHandler, Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
        {
            _container.Options.EnableAutoVerification = false;
            base.ReplaceKeyedHandling(replaceHandling, registerHandler, getHandler);
        }

        protected override ISlackServiceFactory DefaultServiceFactory() =>
            Configure(DefaultContainer(), _ => { });

        protected override ISlackServiceFactory Configure(Action<SimpleInjectorSlackHandlerConfiguration> configure) =>
            Configure(_container, configure);

        private static Container DefaultContainer() =>
            new() { Options = { DefaultScopedLifestyle = new AsyncScopedLifestyle() } };

        private static ISlackServiceFactory Configure(Container container, Action<SimpleInjectorSlackHandlerConfiguration> configure)
        {
            container.Register<InstanceTracker, SimpleInjectorInstanceTracker>(Lifestyle.Singleton);
            container.AddSlackNet(configure);
            return container.GetInstance<ISlackServiceFactory>();
        }

        protected override T ResolveDependency<T>() => _container.GetInstance<T>();

        protected override InstanceTracker InstanceTracker => _container.GetInstance<InstanceTracker>();

        class SimpleInjectorInstanceTracker : InstanceTracker
        {
            private readonly Container _container;
            public SimpleInjectorInstanceTracker(Container container) => _container = container;

            public override void AddInstance(TrackedClass instance)
            {
                if (!_container.IsVerifying)
                    base.AddInstance(instance);
            }
        }
    }
}