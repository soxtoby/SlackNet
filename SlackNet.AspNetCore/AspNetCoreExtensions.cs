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
            serviceCollection.AddSingleton<IEventsObservables, EventsObservables>();

            var configuration = new SlackServiceConfiguration(serviceCollection);
            configuration.RegisterEventHandler<IEventsObservables>();
            configure(configuration);
            Default.RegisterServices((serviceType, createService) => serviceCollection.AddTransient(serviceType, c => createService(c.GetService)));

            serviceCollection.TryAddSingleton<ISlackRequestHandler, SlackRequestHandler>();
            serviceCollection.TryAddSingleton<IEventHandler, CompositeEventHandler>();
            serviceCollection.TryAddSingleton<IBlockActionHandler, CompositeBlockActionHandler>();
            serviceCollection.TryAddSingleton<IBlockOptionProvider, SwitchingBlockOptionProvider>();
            serviceCollection.TryAddSingleton<IInteractiveMessageHandler, SwitchingInteractiveMessageHandler>();
            serviceCollection.TryAddSingleton<IMessageShortcutHandler, CompositeMessageShortcutHandler>();
            serviceCollection.TryAddSingleton<IOptionProvider, SwitchingOptionProvider>();
            serviceCollection.TryAddSingleton<IViewSubmissionHandler, SwitchingViewSubmissionHandler>();
            serviceCollection.TryAddSingleton<ISlashCommandHandler, SwitchingSlashCommandHandler>();
            serviceCollection.TryAddSingleton<IDialogSubmissionHandler, SwitchingDialogSubmissionHandler>();
            serviceCollection.AddTransient<ISlackApiClient>(c => new SlackApiClient(c.GetService<IHttp>(), c.GetService<ISlackUrlBuilder>(), c.GetService<SlackJsonSettings>(), configuration.ApiToken));
            
            return serviceCollection;
        }

        public static IApplicationBuilder UseSlackNet(this IApplicationBuilder app, Action<SlackEndpointConfiguration> configure = null)
        {
            var config = new SlackEndpointConfiguration();
            configure?.Invoke(config);
            return app.UseMiddleware<SlackRequestMiddleware>(config);
        }
    }
}
