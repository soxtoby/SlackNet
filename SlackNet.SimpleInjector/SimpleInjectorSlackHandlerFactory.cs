using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using Container = SimpleInjector.Container;

namespace SlackNet.SimpleInjector
{
    class SimpleInjectorSlackHandlerFactory : ISlackHandlerFactory
    {
        private readonly Container _container;
        public SimpleInjectorSlackHandlerFactory(Container container) => _container = container;

        public IEventHandler CreateEventHandler(SlackRequestContext context) => _container.GetInstance<IEventHandler>();
        public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => _container.GetInstance<IAsyncBlockActionHandler>();
        public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => _container.GetInstance<IBlockOptionProvider>();
        public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => _container.GetInstance<IAsyncMessageShortcutHandler>();
        public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => _container.GetInstance<IAsyncGlobalShortcutHandler>();
        public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => _container.GetInstance<IAsyncViewSubmissionHandler>();
        public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => _container.GetInstance<IAsyncSlashCommandHandler>();
        public IAsyncWorkflowStepEditHandler CreateWorkflowStepEditHandler(SlackRequestContext context) => _container.GetInstance<IAsyncWorkflowStepEditHandler>();

        public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => _container.GetInstance<IInteractiveMessageHandler>();
        public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => _container.GetInstance<IOptionProvider>();
        public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => _container.GetInstance<IDialogSubmissionHandler>();
    }
}