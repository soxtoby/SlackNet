using System;
using SimpleInjector;

namespace SlackNet.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddSlackNet(this Container container, Action<SimpleInjectorSlackServiceConfiguration> configure = null) =>
            SimpleInjectorSlackServiceConfiguration.Configure(container, configure);

        public static ISlackServiceProvider SlackServices(this Container container) => container.GetInstance<ISlackServiceProvider>();
    }
}
