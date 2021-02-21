using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using Autofac;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;

namespace SlackNet.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static void AddSlackNet(this ContainerBuilder containerBuilder, Action<AutofacSlackHandlerConfiguration> configure = null)
        {
            var config = new AutofacSlackHandlerConfiguration(containerBuilder);

            containerBuilder.RegisterType<AutofacSlackServiceFactory>().As<ISlackServiceFactory>().SingleInstance();
            containerBuilder.RegisterType<HttpClient>().SingleInstance();
            containerBuilder.Register(c => Default.Http(c.Resolve<SlackJsonSettings>(), c.Resolve<Func<HttpClient>>())).As<IHttp>().SingleInstance();
            containerBuilder.Register(c => Default.UrlBuilder(c.Resolve<SlackJsonSettings>())).As<ISlackUrlBuilder>().SingleInstance();
            containerBuilder.Register(c => Default.JsonSettings(c.Resolve<ISlackTypeResolver>())).As<SlackJsonSettings>().SingleInstance();
            containerBuilder.Register(c => Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes)).As<ISlackTypeResolver>().SingleInstance();
            containerBuilder.Register(c => Default.WebSocketFactory).As<IWebSocketFactory>().SingleInstance();
            containerBuilder.Register(c => Default.Scheduler).As<IScheduler>().SingleInstance();

            containerBuilder.RegisterType<AutofacSlackRequestListener>().As<ISlackRequestListener>().SingleInstance();
            containerBuilder.RegisterType<AutofacSlackHandlerFactory>().As<ISlackHandlerFactory>().InstancePerLifetimeScope();

            containerBuilder.RegisterGeneric(typeof(AutofacHandlerIndex<>)).As(typeof(IHandlerIndex<>));
            containerBuilder.RegisterComposite<CompositeEventHandler, IEventHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterComposite<CompositeBlockActionHandler, IAsyncBlockActionHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingBlockOptionProvider>().As<IBlockOptionProvider>().InstancePerLifetimeScope();
            containerBuilder.RegisterComposite<CompositeMessageShortcutHandler, IAsyncMessageShortcutHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterComposite<CompositeGlobalShortcutHandler, IAsyncGlobalShortcutHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingOptionProvider>().As<IOptionProvider>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingViewSubmissionHandler>().As<IAsyncViewSubmissionHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingSlashCommandHandler>().As<IAsyncSlashCommandHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingInteractiveMessageHandler>().As<IInteractiveMessageHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SwitchingDialogSubmissionHandler>().As<IDialogSubmissionHandler>().InstancePerLifetimeScope();
            containerBuilder.RegisterComposite<CompositeWorkflowStepEditHandler, IAsyncWorkflowStepEditHandler>().InstancePerLifetimeScope();

            containerBuilder
                .Register(c => new SlackApiClient(c.Resolve<IHttp>(), c.Resolve<ISlackUrlBuilder>(), c.Resolve<SlackJsonSettings>(), config.ApiToken))
                .As<ISlackApiClient>()
                .SingleInstance();

            containerBuilder.Register(c => new CoreSocketModeClient(
                    c.Resolve<ISlackApiClient>().WithAccessToken(config.AppLevelToken),
                    c.Resolve<IWebSocketFactory>(),
                    c.Resolve<SlackJsonSettings>(),
                    c.Resolve<IScheduler>()))
                .As<ICoreSocketModeClient>()
                .SingleInstance();

            containerBuilder.RegisterType<SlackSocketModeClient>()
                .As<ISlackSocketModeClient>()
                .SingleInstance();

            configure?.Invoke(config);
        }
    }
}
