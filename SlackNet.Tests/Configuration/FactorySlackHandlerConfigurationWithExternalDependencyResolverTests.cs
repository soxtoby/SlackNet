using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;

namespace SlackNet.Tests.Configuration
{
    public abstract class FactorySlackHandlerConfigurationWithExternalDependencyResolverTests<TConfig> : FactorySlackHandlerConfigurationTests<TConfig> where TConfig : FactorySlackHandlerConfigurationWithExternalDependencyResolver<TConfig>
    {
        [Test]
        public void RegisterEventHandlerFactory()
        {
            RegisterEventHandler(
                registerGenericHandler: c => c.RegisterEventHandler(() => new TestEventHandler(ResolveDependency<InstanceTracker>())),
                registerTypedHandler: c => c.RegisterEventHandler(() => new TestEventHandler<Hello>(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterBlockActionHandlerFactory()
        {
            RegisterBlockActionHandler(
                registerKeyedButtonHandler: (c, key) => c.RegisterBlockActionHandler(key, () => new TestBlockActionHandler<ButtonAction>(ResolveDependency<InstanceTracker>())),
                registerButtonHandler: c => c.RegisterBlockActionHandler(() => new TestBlockActionHandler<DatePickerAction>(ResolveDependency<InstanceTracker>())),
                registerGenericHandler: c => c.RegisterBlockActionHandler(() => new TestBlockActionHandler(ResolveDependency<InstanceTracker>())),
                registerKeyedAsyncDatePickerHandler: (c, key) => c.RegisterAsyncBlockActionHandler(key, () => new TestAsyncBlockActionHandler<ButtonAction>(ResolveDependency<InstanceTracker>())),
                registerAsyncDatePickerHandler: c => c.RegisterAsyncBlockActionHandler(() => new TestAsyncBlockActionHandler<DatePickerAction>(ResolveDependency<InstanceTracker>())),
                registerGenericAsyncHandler: c => c.RegisterAsyncBlockActionHandler(() => new TestAsyncBlockActionHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterBlockOptionProviderFactory()
        {
            RegisterBlockOptionProvider(
                registerProvider: (c, action) => c.RegisterBlockOptionProvider(action, () => new TestBlockOptionProvider(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterMessageShortcutHandlerFactory()
        {
            RegisterMessageShortcutHandler(
                registerHandler: c => c.RegisterMessageShortcutHandler(() => new TestMessageShortcutHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: c => c.RegisterAsyncMessageShortcutHandler(() => new TestAsyncMessageShortcutHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterMessageShortcutHandlerFactory_Keyed()
        {
            RegisterMessageShortcutHandler_Keyed(
                registerHandler: (c, key) => c.RegisterMessageShortcutHandler(key, () => new TestMessageShortcutHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, key) => c.RegisterAsyncMessageShortcutHandler(key, () => new TestAsyncMessageShortcutHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterGlobalShortcutHandlerFactory()
        {
            RegisterGlobalShortcutHandler(
                registerHandler: c => c.RegisterGlobalShortcutHandler(() => new TestGlobalShortcutHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: c => c.RegisterAsyncGlobalShortcutHandler(() => new TestAsyncGlobalShortcutHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterGlobalShortcutHandlerFactory_Keyed()
        {
            RegisterGlobalShortcutHandler_Keyed(
                registerHandler: (c, key) => c.RegisterGlobalShortcutHandler(key, () => new TestGlobalShortcutHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, key) => c.RegisterAsyncGlobalShortcutHandler(key, () => new TestAsyncGlobalShortcutHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterViewSubmissionHandlerFactory_HandleSubmissions()
        {
            RegisterViewSubmissionHandler_HandleSubmissions(
                registerHandler: (c, key) => c.RegisterViewSubmissionHandler(key, () => new TestViewSubmissionHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, key) => c.RegisterAsyncViewSubmissionHandler(key, () => new TestAsyncViewSubmissionHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterViewSubmissionHandlerFactory_HandleCloses()
        {
            RegisterViewSubmissionHandler_HandleCloses(
                registerHandler: (c, key) => c.RegisterViewSubmissionHandler(key, () => new TestViewSubmissionHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, key) => c.RegisterAsyncViewSubmissionHandler(key, () => new TestAsyncViewSubmissionHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterSlashCommandHandlerFactory()
        {
            RegisterSlashCommandHandler(
                registerHandler: (c, command) => c.RegisterSlashCommandHandler(command, () => new TestSlashCommandHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, command) => c.RegisterAsyncSlashCommandHandler(command, () => new TestAsyncSlashCommandHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterWorkflowStepEditHandlerFactory()
        {
            RegisterWorkflowStepEditHandler(
                registerHandler: c => c.RegisterWorkflowStepEditHandler(() => new TestWorkflowStepEditHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: c => c.RegisterAsyncWorkflowStepEditHandler(() => new TestAsyncWorkflowStepEditHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterWorkflowStepEditHandlerFactory_Keyed()
        {
            RegisterWorkflowStepEditHandler_Keyed(
                registerHandler: (c, key) => c.RegisterWorkflowStepEditHandler(key, () => new TestWorkflowStepEditHandler(ResolveDependency<InstanceTracker>())),
                registerAsyncHandler: (c, key) => c.RegisterAsyncWorkflowStepEditHandler(key, () => new TestAsyncWorkflowStepEditHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterInteractiveMessageHandlerFactory()
        {
            RegisterInteractiveMessageHandler(
                registerHandler: (c, action) => c.RegisterInteractiveMessageHandler(action, () => new TestInteractiveMessageHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterOptionProviderFactory()
        {
            RegisterOptionProvider(
                registerHandler: (c, action) => c.RegisterOptionProvider(action, () => new TestOptionProvider(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterDialogSubmissionHandlerFactory_HandleSubmissions()
        {
            RegisterDialogSubmissionHandler_HandleSubmissions(
                registerHandler: (c, key) => c.RegisterDialogSubmissionHandler(key, () => new TestDialogSubmissionHandler(ResolveDependency<InstanceTracker>())));
        }

        [Test]
        public void RegisterDialogSubmissionHandlerFactory_HandleCancellations()
        {
            RegisterDialogSubmissionHandler_HandleCancellations(
                registerHandler: (c, key) => c.RegisterDialogSubmissionHandler(key, () => new TestDialogSubmissionHandler(ResolveDependency<InstanceTracker>())));
        }

        protected abstract T ResolveDependency<T>() where T : class;
    }
}