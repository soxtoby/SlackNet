using System;
using Autofac;
using NUnit.Framework;
using SlackNet.Autofac;
using SlackNet.Handlers;

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

        protected override ISlackServiceProvider Configure(Action<AutofacSlackServiceConfiguration> configure)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(InstanceTracker);
            containerBuilder.AddSlackNet(configure);
            var container = containerBuilder.Build();
            return container.Resolve<ISlackServiceProvider>();
        }

        protected override T ResolveDependency<T>(IComponentContext resolver) => resolver.Resolve<T>();

        protected override InstanceTracker InstanceTracker => _instanceTracker;
    }
}