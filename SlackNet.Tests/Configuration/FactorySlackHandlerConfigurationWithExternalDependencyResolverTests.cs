using NSubstitute;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Tests.Configuration;

public abstract class FactorySlackHandlerConfigurationWithExternalDependencyResolverTests<TConfig> : FactorySlackHandlerConfigurationTests<TConfig> where TConfig : FactorySlackServiceConfigurationWithExternalDependencyResolver<TConfig>
{
    [Test]
    public void UseHttpFactory()
    {
        UseService<IHttp, TestHttp>(
            c => c.UseHttp(() => new TestHttp()),
            s => s.GetHttp());
    }

    [Test]
    public void UseJsonSettingsFactory()
    {
        UseService<SlackJsonSettings, TestJsonSettings>(
            c => c.UseJsonSettings(() => new TestJsonSettings()),
            s => s.GetJsonSettings());
    }

    [Test]
    public void UseFactoryResolverFactory()
    {
        UseService<ISlackTypeResolver, TestTypeResolver>(
            c => c.UseTypeResolver(() => new TestTypeResolver()),
            s => s.GetTypeResolver());
    }

    [Test]
    public void UseUrlBuilderFactory()
    {
        UseService<ISlackUrlBuilder, TestUrlBuilder>(
            c => c.UseUrlBuilder(() => new TestUrlBuilder()),
            s => s.GetUrlBuilder());
    }

    [Test]
    public void UseLoggerFactory()
    {
        UseService<ILogger, TestLogger>(
            c => c.UseLogger(() => new TestLogger()),
            s => s.GetLogger());
    }

    [Test]
    public void UseWebSocketFactoryFactory()
    {
        UseService<IWebSocketFactory, TestWebSocketFactory>(
            c => c.UseWebSocketFactory(() => new TestWebSocketFactory()),
            s => s.GetWebSocketFactory());
    }

    [Test]
    public void UseRequestListenerFactory()
    {
        var sut = Configure(c => c.UseRequestListener(() => new TestRequestListener(InstanceTracker)));

        RequestListenersShouldBeCreatedOnceOnEnumeration(sut, InstanceTracker);
    }

    [Test]
    public void UseHandlerFactoryFactory()
    {
        UseService<ISlackHandlerFactory, TestHandlerFactory>(
            c => c.UseHandlerFactory(() => new TestHandlerFactory()),
            s => s.GetHandlerFactory());
    }

    [Test]
    public void UseApiClientFactory()
    {
        UseService<ISlackApiClient, TestApiClient>(
            c => c.UseApiClient(() => new TestApiClient()),
            s => s.GetApiClient());
    }

    [Test]
    public void UseSocketModeClientFactory()
    {
        UseService<ISlackSocketModeClient, TestSocketModeClient>(
            c => c.UseSocketModeClient(() => new TestSocketModeClient()),
            s => s.GetSocketModeClient());
    }

    [Test]
    public void ReplaceEventHandling_WithFactory()
    {
        ReplaceRequestHandling<IEventHandler, TestEventHandler>(
            c => c.ReplaceEventHandling(() => new TestEventHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateEventHandler(ctx));
    }

    [Test]
    public void ReplaceBlockActionHandling_WithFactory()
    {
        ReplaceRequestHandling<IAsyncBlockActionHandler, TestAsyncBlockActionHandler>(
            c => c.ReplaceBlockActionHandling(() => new TestAsyncBlockActionHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateBlockActionHandler(ctx));
    }

    [Test]
    public void ReplaceBlockOptionProvider_WithFactory()
    {
        ReplaceRequestHandling<IBlockOptionProvider, TestBlockOptionProvider>(
            c => c.ReplaceBlockOptionProviding(() => new TestBlockOptionProvider(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateBlockOptionProvider(ctx));
    }

    [Test]
    public void ReplaceMessageShortcutHandling_WithFactory()
    {
        ReplaceRequestHandling<IAsyncMessageShortcutHandler, TestAsyncMessageShortcutHandler>(
            c => c.ReplaceMessageShortcutHandling(() => new TestAsyncMessageShortcutHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateMessageShortcutHandler(ctx));
    }

    [Test]
    public void ReplaceGlobalShortcutHandling_WithFactory()
    {
        ReplaceRequestHandling<IAsyncGlobalShortcutHandler, TestAsyncGlobalShortcutHandler>(
            c => c.ReplaceGlobalShortcutHandling(() => new TestAsyncGlobalShortcutHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx));
    }

    [Test]
    public void ReplaceViewSubmissionHandling_WithFactory()
    {
        ReplaceRequestHandling<IAsyncViewSubmissionHandler, TestAsyncViewSubmissionHandler>(
            c => c.ReplaceViewSubmissionHandling(() => new TestAsyncViewSubmissionHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateViewSubmissionHandler(ctx));
    }

    [Test]
    public void ReplaceSlashCommandHandling_WithFactory()
    {
        ReplaceRequestHandling<IAsyncSlashCommandHandler, TestAsyncSlashCommandHandler>(
            c => c.ReplaceSlashCommandHandling(() => new TestAsyncSlashCommandHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateSlashCommandHandler(ctx));
    }

    [Test]
    public void ReplaceLegacyInteractiveMessageHandling_WithFactory()
    {
        ReplaceRequestHandling<IInteractiveMessageHandler, TestInteractiveMessageHandler>(
            c => c.ReplaceLegacyInteractiveMessageHandling(() => new TestInteractiveMessageHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx));
    }

    [Test]
    public void ReplaceLegacyOptionProviding_WithFactory()
    {
        ReplaceRequestHandling<IOptionProvider, TestOptionProvider>(
            c => c.ReplaceLegacyOptionProviding(() => new TestOptionProvider(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateLegacyOptionProvider(ctx));
    }

    [Test]
    public void ReplaceLegacyDialogSubmissionHandling_WithFactory()
    {
        ReplaceRequestHandling<IDialogSubmissionHandler, TestDialogSubmissionHandler>(
            c => c.ReplaceLegacyDialogSubmissionHandling(() => new TestDialogSubmissionHandler(ResolveDependency<InstanceTracker>())),
            (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx));
    }

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
    public void RegisterSlashCommandHandlerFactory_InvalidCommandName_Throws()
    {
        RegisterSlashCommandHandlerWithInvalidCommandName(
            c => c.RegisterSlashCommandHandler("foo", () => Substitute.For<ISlashCommandHandler>()),
            c => c.RegisterAsyncSlashCommandHandler("foo", () => Substitute.For<IAsyncSlashCommandHandler>()));
    }

    [Test]
    public void RegisterSlashCommandHandlerFactory()
    {
        RegisterSlashCommandHandler(
            registerHandler: (c, command) => c.RegisterSlashCommandHandler(command, () => new TestSlashCommandHandler(ResolveDependency<InstanceTracker>())),
            registerAsyncHandler: (c, command) => c.RegisterAsyncSlashCommandHandler(command, () => new TestAsyncSlashCommandHandler(ResolveDependency<InstanceTracker>())));
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