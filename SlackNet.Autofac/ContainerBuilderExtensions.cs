using System;
using Autofac;

namespace SlackNet.Autofac;

public static class ContainerBuilderExtensions
{
    public static void AddSlackNet(this ContainerBuilder containerBuilder, Action<AutofacSlackServiceConfiguration> configure = null) =>
        AutofacSlackServiceConfiguration.Configure(containerBuilder, configure);
}