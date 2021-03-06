using System;
using Autofac;

namespace SlackNet.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void AddSlackNet(this ContainerBuilder containerBuilder, Action<AutofacSlackHandlerConfiguration> configure = null) =>
            AutofacSlackHandlerConfiguration.Configure(containerBuilder, configure);
    }
}
