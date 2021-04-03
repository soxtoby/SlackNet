using System;
using EasyAssertions;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SlackNet.SimpleInjector;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class SimpleInjectorTests : FactorySlackHandlerConfigurationWithExternalDependencyResolverTests<SimpleInjectorSlackServiceConfiguration>
    {
        private Container _container;

        protected override void ResolvedServiceShouldReferToProviderService<TService>(Func<ISlackServiceProvider, TService> getServiceFromProvider)
        {
            var container = DefaultContainer();
            var slackServiceProvider = Configure(container, _ => { });

            container.GetInstance<TService>()
                .ShouldReferTo(getServiceFromProvider(slackServiceProvider));
        }

        protected override ISlackServiceProvider DefaultServiceFactory() =>
            Configure(DefaultContainer(), _ => { });

        protected override ISlackServiceProvider Configure(Action<SimpleInjectorSlackServiceConfiguration> configure) =>
            Configure(_container = DefaultContainer(), configure);

        private static Container DefaultContainer() =>
            new() { Options = { DefaultScopedLifestyle = new AsyncScopedLifestyle() } };

        private static ISlackServiceProvider Configure(Container container, Action<SimpleInjectorSlackServiceConfiguration> configure)
        {
            container.Register<InstanceTracker, SimpleInjectorInstanceTracker>(Lifestyle.Singleton);
            container.AddSlackNet(configure);
            return container.GetInstance<ISlackServiceProvider>();
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