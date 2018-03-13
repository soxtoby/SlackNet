using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<SlackServiceConfiguration> configure)
        {
            var configuration = new SlackServiceConfiguration(serviceCollection);
            configure(configuration);
            Default.RegisterServices((serviceType, createService) => serviceCollection.AddTransient(serviceType, c => createService(c.GetService)));
            serviceCollection.AddSingleton<ISlackEvents, SlackEventsService>();
            serviceCollection.AddSingleton<ISlackActions, SlackActionsService>();
            serviceCollection.AddSingleton<ISlackOptions, SlackOptionsService>();
            serviceCollection.TryAddSingleton<IDialogSubmissionHandler, NullDialogSubmissionHandler>();
            serviceCollection.AddTransient<ISlackApiClient>(c => new SlackApiClient(c.GetService<IHttp>(), c.GetService<ISlackUrlBuilder>(), c.GetService<SlackJsonSettings>(), configuration.ApiToken));
            
            return serviceCollection;
        }

        public static IApplicationBuilder UseSlackNet(this IApplicationBuilder app, Action<SlackEndpointConfiguration> configure = null)
        {
            var config = new SlackEndpointConfiguration();
            configure?.Invoke(config);
            return app.UseMiddleware<SlackEventsMiddleware>(config);
        }
    }
}
