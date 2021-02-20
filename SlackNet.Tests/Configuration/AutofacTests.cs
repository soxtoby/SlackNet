using System;
using Autofac;
using NUnit.Framework;
using SlackNet.Autofac;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class AutofacTests : FactorySlackHandlerConfigurationWithDependencyResolverTests<AutofacSlackHandlerConfiguration, IComponentContext>
    {
        private InstanceTracker _instanceTracker;

        [SetUp]
        public void SetUp()
        {
            _instanceTracker = new InstanceTracker();
        }

        protected override ISlackServiceFactory Configure(Action<AutofacSlackHandlerConfiguration> configure)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(InstanceTracker);
            containerBuilder.AddSlackNet(configure);
            var container = containerBuilder.Build();
            return container.Resolve<ISlackServiceFactory>();
        }

        protected override T ResolveDependency<T>(IComponentContext resolver) => resolver.Resolve<T>();

        protected override InstanceTracker InstanceTracker => _instanceTracker;
    }
}