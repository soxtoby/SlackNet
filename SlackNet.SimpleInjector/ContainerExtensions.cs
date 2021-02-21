using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using SimpleInjector;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;
using Container = SimpleInjector.Container;

namespace SlackNet.SimpleInjector
{
    public static class ContainerExtensions
    {
        public static void AddSlackNet(this Container container, Action<SimpleInjectorSlackHandlerConfiguration> configure = null)
        {
            var config = new SimpleInjectorSlackHandlerConfiguration(container);
            configure?.Invoke(config);

            container.RegisterFallbackType<ISlackServiceFactory, SimpleInjectorSlackServiceFactory>(Lifestyle.Singleton);
            container.RegisterFallback(() => new HttpClient(), Lifestyle.Singleton);
            container.RegisterFallback(() => Default.Http(container.GetInstance<SlackJsonSettings>(), container.GetInstance<HttpClient>), Lifestyle.Singleton);
            container.RegisterFallback(() => Default.UrlBuilder(container.GetInstance<SlackJsonSettings>()), Lifestyle.Singleton);
            container.RegisterFallback(() => Default.JsonSettings(container.GetInstance<ISlackTypeResolver>()), Lifestyle.Singleton);
            container.RegisterFallback(() => Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes), Lifestyle.Singleton);
            container.RegisterFallback(() => Default.WebSocketFactory, Lifestyle.Singleton);
            container.RegisterFallback(() => Default.Scheduler, Lifestyle.Singleton);

            container.RegisterFallbackType<ISlackRequestListener, SimpleInjectorSlackRequestListener>(Lifestyle.Singleton);
            container.RegisterFallbackType<ISlackHandlerFactory, SimpleInjectorSlackHandlerFactory>(Lifestyle.Singleton);

            container.RegisterFallback(() => config.HandlerIndex<IBlockOptionProvider>(), Lifestyle.Singleton);
            container.RegisterFallback(() => config.HandlerIndex<IAsyncViewSubmissionHandler>(), Lifestyle.Singleton);
            container.RegisterFallback(() => config.HandlerIndex<IAsyncSlashCommandHandler>(), Lifestyle.Singleton);
            container.RegisterFallback(() => config.HandlerIndex<IInteractiveMessageHandler>(), Lifestyle.Singleton);
            container.RegisterFallback(() => config.HandlerIndex<IOptionProvider>(), Lifestyle.Singleton);
            container.RegisterFallback(() => config.HandlerIndex<IDialogSubmissionHandler>(), Lifestyle.Singleton);

            // We need to resolve collections of these handlers, but SimpleInjector requires at least one registration.
            container.RegisterFallbackCollection<IEventHandler>();
            container.RegisterFallbackCollection<IAsyncBlockActionHandler>();
            container.RegisterFallbackCollection<IAsyncMessageShortcutHandler>();
            container.RegisterFallbackCollection<IAsyncGlobalShortcutHandler>();
            container.RegisterFallbackCollection<IAsyncWorkflowStepEditHandler>();

            container.RegisterFallbackType<IEventHandler, CompositeEventHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncBlockActionHandler, CompositeBlockActionHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IBlockOptionProvider, SwitchingBlockOptionProvider>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncMessageShortcutHandler, CompositeMessageShortcutHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncGlobalShortcutHandler, CompositeGlobalShortcutHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IOptionProvider, SwitchingOptionProvider>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncViewSubmissionHandler, SwitchingViewSubmissionHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncSlashCommandHandler, SwitchingSlashCommandHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IInteractiveMessageHandler, SwitchingInteractiveMessageHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IDialogSubmissionHandler, SwitchingDialogSubmissionHandler>(Lifestyle.Scoped);
            container.RegisterFallbackType<IAsyncWorkflowStepEditHandler, CompositeWorkflowStepEditHandler>(Lifestyle.Scoped);

            container.RegisterFallback<ISlackApiClient>(() => new SlackApiClient(container.GetInstance<IHttp>(), container.GetInstance<ISlackUrlBuilder>(), container.GetInstance<SlackJsonSettings>(), config.ApiToken), Lifestyle.Singleton);
            container.RegisterFallback<ICoreSocketModeClient>(() => new CoreSocketModeClient(
                    container.GetInstance<ISlackApiClient>().WithAccessToken(config.AppLevelToken),
                    container.GetInstance<IWebSocketFactory>(),
                    container.GetInstance<SlackJsonSettings>(),
                    container.GetInstance<IScheduler>()),
                Lifestyle.Singleton);
            container.RegisterFallbackType<ISlackSocketModeClient, SlackSocketModeClient>(Lifestyle.Singleton);
        }

        internal static void RegisterFallbackType<TService, TImplementation>(this Container container, Lifestyle lifestyle) where TService : class where TImplementation : class, TService =>
            container.RegisterFallback<TService>(a => a.Register(lifestyle.CreateProducer<TService, TImplementation>(container).Registration));

        private static void RegisterFallback<TService>(this Container container, Func<TService> factory, Lifestyle lifestyle) where TService : class =>
            container.RegisterFallback<TService>(a => a.Register(lifestyle.CreateRegistration(factory, container)));

        private static void RegisterFallbackCollection<TService>(this Container container) where TService : class =>
            container.RegisterFallback<IEnumerable<TService>>(a => a.Register(Enumerable.Empty<TService>));

        private static void RegisterFallback<TService>(this Container container, Action<UnregisteredTypeEventArgs> register) =>
            container.ResolveUnregisteredType += (_, args) =>
                {
                    if (!args.Handled && args.UnregisteredServiceType == typeof(TService))
                        register(args);
                };
    }
}
