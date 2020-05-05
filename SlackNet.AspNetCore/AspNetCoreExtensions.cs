using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

            serviceCollection.AddSingleton<ISlackRequestHandler, SlackRequestHandler>();
            serviceCollection.AddSingleton<IEventHandler, CompositeEventHandler>();
            serviceCollection.AddSingleton<IBlockActionHandler, CompositeBlockActionHandler>();
            serviceCollection.AddSingleton<IBlockOptionProvider, SwitchingBlockOptionProvider>();
            serviceCollection.AddSingleton<IInteractiveMessageHandler, SwitchingInteractiveMessageHandler>();
            serviceCollection.AddSingleton<IMessageShortcutHandler, CompositeMessageShortcutHandler>();
            serviceCollection.AddSingleton<IOptionProvider, SwitchingOptionProvider>();
            serviceCollection.AddSingleton<IViewSubmissionHandler, SwitchingViewSubmissionHandler>();
            serviceCollection.AddSingleton<ISlashCommandHandler, SwitchingSlashCommandHandler>();
            serviceCollection.AddSingleton<IDialogSubmissionHandler, SwitchingDialogSubmissionHandler>();
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
