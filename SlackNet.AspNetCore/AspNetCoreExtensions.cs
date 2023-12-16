using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SlackNet.AspNetCore;

public static class AspNetCoreExtensions
{
    public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<AspNetSlackServiceConfiguration> configure = null)
    {
        var config = new AspNetSlackServiceConfiguration(serviceCollection);
        configure?.Invoke(config);
        config.ConfigureServices();
        return serviceCollection;
    }

    /// <summary>
    /// Adds the Slack request-handling middleware to ASP.NET.
    /// By default, the following routes are configured:
    /// <br /><c>/slack/event</c> - Event subscriptions
    /// <br /><c>/slack/action</c> - Interactive component requests
    /// <br /><c>/slack/options</c> - Options loading (for message menus)
    /// <br /><c>/slack/command</c> - Slash command requests
    /// </summary>
    public static IApplicationBuilder UseSlackNet(this IApplicationBuilder app, Action<SlackEndpointConfiguration> configure = null)
    {
        var config = new SlackEndpointConfiguration();
        configure?.Invoke(config);

        if (config.SocketMode)
        {
            app.ApplicationServices.GetRequiredService<ISlackSocketModeClient>().Connect();
            return app;
        }
        else
        {
            return app.UseMiddleware<SlackRequestMiddleware>(config);
        }
    }
}