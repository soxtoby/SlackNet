using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;

namespace SlackNet.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<ServiceCollectionSlackServiceConfiguration> configure = null)
    {
        ServiceCollectionSlackServiceConfiguration.Configure(serviceCollection, configure);
        return serviceCollection;
    }
}