using Microsoft.Extensions.DependencyInjection;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Extensions.DependencyInjection
{
    class ServiceProviderSlackHandlerFactory : ISlackHandlerFactory
    {
        public IEventHandler CreateEventHandler(SlackRequestContext context) => Resolve<IEventHandler>(context);
        public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => Resolve<IAsyncBlockActionHandler>(context);
        public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => Resolve<IBlockOptionProvider>(context);
        public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => Resolve<IAsyncMessageShortcutHandler>(context);
        public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => Resolve<IAsyncGlobalShortcutHandler>(context);
        public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => Resolve<IAsyncViewSubmissionHandler>(context);
        public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => Resolve<IAsyncSlashCommandHandler>(context);
        public IAsyncWorkflowStepEditHandler CreateWorkflowStepEditHandler(SlackRequestContext context) => Resolve<IAsyncWorkflowStepEditHandler>(context);

        public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => Resolve<IInteractiveMessageHandler>(context);
        public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => Resolve<IOptionProvider>(context);
        public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => Resolve<IDialogSubmissionHandler>(context);

        private static THandler Resolve<THandler>(SlackRequestContext context) => context.ServiceScope().ServiceProvider.GetRequiredService<THandler>();
    }
}