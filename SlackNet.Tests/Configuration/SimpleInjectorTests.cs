using System;
using EasyAssertions;
using NUnit.Framework;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using SlackNet.SimpleInjector;

namespace SlackNet.Tests.Configuration;

[TestFixture]
public class SimpleInjectorTests : FactorySlackHandlerConfigurationWithExternalDependencyResolverTests<SimpleInjectorSlackServiceConfiguration>
{
    private Container _container;
    private InstanceTracker _instanceTracker;

    [SetUp]
    public void SetUp()
    {
        _instanceTracker = new InstanceTracker();
    }

    [TearDown]
    public void TearDown()
    {
        _container?.Verify();
    }

    protected override void ResolvedServiceShouldReferToProviderService<TService>(Func<ISlackServiceProvider, TService> getServiceFromProvider)
    {
        var container = DefaultContainer();
        var slackServiceProvider = Configure(container, _ => { });

        container.GetInstance<TService>()
            .ShouldReferTo(getServiceFromProvider(slackServiceProvider));
    }

    protected override ISlackServiceProvider Configure(Action<SimpleInjectorSlackServiceConfiguration> configure) =>
        Configure(_container = DefaultContainer(), configure);

    private static Container DefaultContainer() =>
        new() { Options = { DefaultScopedLifestyle = new AsyncScopedLifestyle(), EnableAutoVerification = false} };

    private ISlackServiceProvider Configure(Container container, Action<SimpleInjectorSlackServiceConfiguration> configure)
    {
        container.RegisterInstance(_instanceTracker);
        container.AddSlackNet(configure);
        return container.GetInstance<ISlackServiceProvider>();
    }

    protected override T ResolveDependency<T>() => _container.GetInstance<T>();

    protected override InstanceTracker InstanceTracker => _instanceTracker;
}