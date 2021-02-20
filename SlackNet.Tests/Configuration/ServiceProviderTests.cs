using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SlackNet.Extensions.DependencyInjection;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    [TestFixture]
    public class ServiceProviderTests : FactorySlackHandlerConfigurationWithDependencyResolverTests<SlackHandlerServiceConfiguration, IServiceProvider>
    {
        private InstanceTracker _instanceTracker;

        [SetUp]
        public void SetUp()
        {
            _instanceTracker = new InstanceTracker();
        }

        protected override ISlackServiceFactory Configure(Action<SlackHandlerServiceConfiguration> configure)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(InstanceTracker);
            serviceCollection.AddSlackNet(configure);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ISlackServiceFactory>();
        }

        protected override T ResolveDependency<T>(IServiceProvider resolver) => resolver.GetRequiredService<T>();

        protected override InstanceTracker InstanceTracker => _instanceTracker;
    }
}