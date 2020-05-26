using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<SlackServiceConfiguration> configure)
        {
            serviceCollection.AddSingleton<IEventsObservables, EventsObservables>();

            var configuration = new SlackServiceConfiguration(serviceCollection);
            configuration.RegisterEventHandler<IEventsObservables>();
            configure(configuration);
            Default.RegisterServices((serviceType, createService) => serviceCollection.AddTransient(serviceType, c => createService(c.GetService)));

            serviceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
            serviceCollection.TryAddSingleton<IEventHandler, CompositeEventHandler>();
            serviceCollection.TryAddSingleton<IAsyncBlockActionHandler, CompositeBlockActionHandler>();
            serviceCollection.TryAddSingleton<IBlockOptionProvider, SwitchingBlockOptionProvider>();
            serviceCollection.TryAddSingleton<IAsyncMessageShortcutHandler, CompositeMessageShortcutHandler>();
            serviceCollection.TryAddSingleton<IAsyncGlobalShortcutHandler, CompositeGlobalShortcutHandler>();
            serviceCollection.TryAddSingleton<IOptionProvider, SwitchingOptionProvider>();
            serviceCollection.TryAddSingleton<IAsyncViewSubmissionHandler, SwitchingViewSubmissionHandler>();
            serviceCollection.TryAddSingleton<IAsyncSlashCommandHandler, SwitchingSlashCommandHandler>();
            serviceCollection.TryAddSingleton<IInteractiveMessageHandler, SwitchingInteractiveMessageHandler>();
            serviceCollection.TryAddSingleton<IDialogSubmissionHandler, SwitchingDialogSubmissionHandler>();
            serviceCollection.AddTransient<ISlackApiClient>(c => new SlackApiClient(c.GetService<IHttp>(), c.GetService<ISlackUrlBuilder>(), c.GetService<SlackJsonSettings>(), configuration.ApiToken));
            
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
            return app.UseMiddleware<SlackRequestMiddleware>(config);
        }
    }
}
