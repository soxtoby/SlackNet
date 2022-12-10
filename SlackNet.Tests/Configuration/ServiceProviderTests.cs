using System;
using System.Reflection;
using EasyAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SlackNet.Extensions.DependencyInjection;

namespace SlackNet.Tests.Configuration;

[TestFixture]
public class ServiceProviderTests : FactorySlackHandlerConfigurationWithDependencyResolverTests<ServiceCollectionSlackServiceConfiguration, IServiceProvider>
{
    private InstanceTracker _instanceTracker;

    [SetUp]
    public void SetUp()
    {
        _instanceTracker = new InstanceTracker();
    }

    protected override void ResolvedServiceShouldReferToProviderService<TService>(Func<ISlackServiceProvider, TService> getServiceFromProvider)
    {
        var serviceProvider = ConfigureServiceProvider(_ => { });
        var slackServiceProvider = serviceProvider.GetRequiredService<ISlackServiceProvider>();

        serviceProvider.GetRequiredService<TService>()
            .ShouldReferTo(getServiceFromProvider(slackServiceProvider));
    }

    protected override ISlackServiceProvider Configure(Action<ServiceCollectionSlackServiceConfiguration> configure)
    {
        return ConfigureServiceProvider(configure).GetRequiredService<ISlackServiceProvider>();
    }

    private ServiceProvider ConfigureServiceProvider(Action<ServiceCollectionSlackServiceConfiguration> configure)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(InstanceTracker);
        serviceCollection.AddSlackNet(configure);
        return serviceCollection.BuildServiceProvider();
    }

    protected override T ResolveDependency<T>(IServiceProvider resolver) => resolver.GetRequiredService<T>();

    protected override InstanceTracker InstanceTracker => _instanceTracker;
}