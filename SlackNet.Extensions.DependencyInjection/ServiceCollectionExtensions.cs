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

namespace SlackNet.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackNet(this IServiceCollection serviceCollection, Action<SlackHandlerServiceConfiguration> configure = null)
        {
            var config = new SlackHandlerServiceConfiguration(serviceCollection);
            configure?.Invoke(config);

            serviceCollection.TryAddSingleton<ISlackServiceFactory, ServiceProviderSlackServiceFactory>();
            serviceCollection.TryAddSingleton<HttpClient>();
            serviceCollection.TryAddSingleton(sp => Default.Http(sp.GetRequiredService<SlackJsonSettings>(), sp.GetRequiredService<HttpClient>));
            serviceCollection.TryAddSingleton(sp => Default.UrlBuilder(sp.GetRequiredService<SlackJsonSettings>()));
            serviceCollection.TryAddSingleton(sp => Default.JsonSettings(sp.GetRequiredService<ISlackTypeResolver>()));
            serviceCollection.TryAddSingleton(sp => Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            serviceCollection.TryAddSingleton(sp => Default.WebSocketFactory);
            serviceCollection.TryAddSingleton(sp => Default.Scheduler);

            serviceCollection.TryAddSingleton<ISlackRequestListener, ServiceProviderSlackRequestListener>();
            serviceCollection.TryAddScoped<ISlackHandlerFactory, ServiceProviderSlackHandlerFactory>();

            serviceCollection.TryAddScoped(typeof(IHandlerIndex<>), typeof(KeyedHandlerIndex<>));
            serviceCollection.TryAddScoped<IEventHandler>(sp => new CompositeEventHandler(sp.GetRequiredService<IEnumerable<CollectionItem<IEventHandler>>>().Select(c => c.Item)));
            serviceCollection.TryAddScoped<IAsyncBlockActionHandler>(sp => new CompositeBlockActionHandler(sp.GetRequiredService<IEnumerable<CollectionItem<IAsyncBlockActionHandler>>>().Select(c => c.Item)));
            serviceCollection.TryAddScoped<IBlockOptionProvider>(sp => new SwitchingBlockOptionProvider(sp.GetRequiredService<IHandlerIndex<IBlockOptionProvider>>()));
            serviceCollection.TryAddScoped<IAsyncMessageShortcutHandler>(sp => new CompositeMessageShortcutHandler(sp.GetRequiredService<IEnumerable<CollectionItem<IAsyncMessageShortcutHandler>>>().Select(c => c.Item)));
            serviceCollection.TryAddScoped<IAsyncGlobalShortcutHandler>(sp => new CompositeGlobalShortcutHandler(sp.GetRequiredService<IEnumerable<CollectionItem<IAsyncGlobalShortcutHandler>>>().Select(c => c.Item)));
            serviceCollection.TryAddScoped<IOptionProvider>(sp => new SwitchingOptionProvider(sp.GetRequiredService<IHandlerIndex<IOptionProvider>>()));
            serviceCollection.TryAddScoped<IAsyncViewSubmissionHandler>(sp => new SwitchingViewSubmissionHandler(sp.GetRequiredService<IHandlerIndex<IAsyncViewSubmissionHandler>>()));
            serviceCollection.TryAddScoped<IAsyncSlashCommandHandler>(sp => new SwitchingSlashCommandHandler(sp.GetRequiredService<IHandlerIndex<IAsyncSlashCommandHandler>>()));
            serviceCollection.TryAddScoped<IInteractiveMessageHandler>(sp => new SwitchingInteractiveMessageHandler(sp.GetRequiredService<IHandlerIndex<IInteractiveMessageHandler>>()));
            serviceCollection.TryAddScoped<IDialogSubmissionHandler>(sp => new SwitchingDialogSubmissionHandler(sp.GetRequiredService<IHandlerIndex<IDialogSubmissionHandler>>()));
            serviceCollection.TryAddScoped<IAsyncWorkflowStepEditHandler>(sp => new CompositeWorkflowStepEditHandler(sp.GetRequiredService<IEnumerable<CollectionItem<IAsyncWorkflowStepEditHandler>>>().Select(c => c.Item)));

            serviceCollection.TryAddSingleton<ISlackApiClient>(sp => new SlackApiClient(sp.GetRequiredService<IHttp>(), sp.GetRequiredService<ISlackUrlBuilder>(), sp.GetRequiredService<SlackJsonSettings>(), config.ApiToken));
            serviceCollection.TryAddSingleton<ICoreSocketModeClient>(sp => new CoreSocketModeClient(
                sp.GetRequiredService<ISlackApiClient>().WithAccessToken(config.AppLevelToken),
                sp.GetRequiredService<IWebSocketFactory>(),
                sp.GetRequiredService<SlackJsonSettings>(),
                sp.GetRequiredService<IScheduler>()));
            serviceCollection.TryAddSingleton<ISlackSocketModeClient, SlackSocketModeClient>();

            return serviceCollection;
        }
    }
}
