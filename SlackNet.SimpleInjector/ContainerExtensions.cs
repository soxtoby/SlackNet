using System;
using SimpleInjector;

namespace SlackNet.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddSlackNet(this Container container, Action<SimpleInjectorSlackHandlerConfiguration> configure = null) =>
            SimpleInjectorSlackHandlerConfiguration.Configure(container, configure);
    }
}
