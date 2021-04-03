using System;
using Autofac;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Autofac;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class AutofacTests : FactorySlackHandlerConfigurationWithDependencyResolverTests<AutofacSlackServiceConfiguration, IComponentContext>
    {
        private InstanceTracker _instanceTracker;

        [SetUp]
        public void SetUp()
        {
            _instanceTracker = new InstanceTracker();
        }

        protected override void ResolvedServiceShouldReferToProviderService<TService>(Func<ISlackServiceProvider, TService> getServiceFromProvider)
        {
            var container = ConfigureContainer(_ => { });
            var slackServiceProvider = container.Resolve<ISlackServiceProvider>();

            container.Resolve<TService>()
                .ShouldReferTo(getServiceFromProvider(slackServiceProvider));
        }

        protected override ISlackServiceProvider Configure(Action<AutofacSlackServiceConfiguration> configure) =>
            ConfigureContainer(configure).Resolve<ISlackServiceProvider>();

        private IContainer ConfigureContainer(Action<AutofacSlackServiceConfiguration> configure)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(InstanceTracker);
            containerBuilder.AddSlackNet(configure);
            return containerBuilder.Build();
        }

        protected override T ResolveDependency<T>(IComponentContext resolver) => resolver.Resolve<T>();

        protected override InstanceTracker InstanceTracker => _instanceTracker;
    }
}