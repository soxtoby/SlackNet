using System;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static ISlackServiceProvider SlackServices(this IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<ISlackServiceProvider>();
    }
}