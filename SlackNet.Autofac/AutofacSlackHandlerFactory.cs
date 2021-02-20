using Autofac;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Autofac
{
    class AutofacSlackHandlerFactory : ISlackHandlerFactory
    {
        public IEventHandler CreateEventHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IEventHandler>();
        public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncBlockActionHandler>();
        public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => context.LifetimeScope().Resolve<IBlockOptionProvider>();
        public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncMessageShortcutHandler>();
        public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncGlobalShortcutHandler>();
        public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncViewSubmissionHandler>();
        public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncSlashCommandHandler>();
        public IAsyncWorkflowStepEditHandler CreateWorkflowStepEditHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IAsyncWorkflowStepEditHandler>();

        public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IInteractiveMessageHandler>();
        public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => context.LifetimeScope().Resolve<IOptionProvider>();
        public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => context.LifetimeScope().Resolve<IDialogSubmissionHandler>(); }
}