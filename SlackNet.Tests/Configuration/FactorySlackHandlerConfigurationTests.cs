using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.SocketMode;
using SlackNet.WebApi;
using Hello = SlackNet.Events.Hello;

namespace SlackNet.Tests.Configuration;

public abstract class FactorySlackHandlerConfigurationTests<TConfig> : SlackServiceConfigurationBaseTests<TConfig> where TConfig : FactorySlackServiceConfiguration<TConfig>
{
    protected abstract InstanceTracker InstanceTracker { get; }
    private const string SameInstanceForSameRequest = "Should be same instance within request";
    private const string DifferentInstanceForDifferentRequest = "Should be different instance for different request";

    [TestCaseSource(nameof(SlackServiceProviderMethods))]
    public void SlackNetServicesAreRegistered(MethodInfo providerMethod)
    {
        var assertion = GetType()
            .GetMethod(nameof(ResolvedServiceShouldReferToProviderService), BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(providerMethod.ReturnType);
        var getServiceFromProvider = providerMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(ISlackServiceProvider), providerMethod.ReturnType));
        assertion.Invoke(this, new object[] { getServiceFromProvider });
    }

    protected abstract void ResolvedServiceShouldReferToProviderService<TService>(Func<ISlackServiceProvider, TService> getServiceFromProvider) where TService : class;

    [Test]
    public void UseHttpType()
    {
        UseService<IHttp, TestHttp>(
            c => c.UseHttp<TestHttp>(),
            s => s.GetHttp());
    }

    [Test]
    public void UseJsonSettingsType()
    {
        UseService<SlackJsonSettings, TestJsonSettings>(
            c => c.UseJsonSettings<TestJsonSettings>(),
            s => s.GetJsonSettings());
    }

    [Test]
    public void UseTypeResolverType()
    {
        UseService<ISlackTypeResolver, TestTypeResolver>(
            c => c.UseTypeResolver<TestTypeResolver>(),
            s => s.GetTypeResolver());
    }

    [Test]
    public void UseUrlBuilderType()
    {
        UseService<ISlackUrlBuilder, TestUrlBuilder>(
            c => c.UseUrlBuilder<TestUrlBuilder>(),
            s => s.GetUrlBuilder());
    }

    [Test]
    public void UseLoggerType()
    {
        UseService<ILogger, TestLogger>(
            c => c.UseLogger<TestLogger>(),
            s => s.GetLogger());
    }

    [Test]
    public void UseWebSocketFactoryType()
    {
        UseService<IWebSocketFactory, TestWebSocketFactory>(
            c => c.UseWebSocketFactory<TestWebSocketFactory>(),
            s => s.GetWebSocketFactory());
    }

    [Test]
    public void UseRequestListenerType()
    {
        var sut = Configure(c => c.UseRequestListener<TestRequestListener>());

        RequestListenersShouldBeCreatedOnceOnEnumeration(sut, InstanceTracker);
    }

    protected static void RequestListenersShouldBeCreatedOnceOnEnumeration(ISlackServiceProvider sut, InstanceTracker instanceTracker)
    {
        var listeners = sut.GetRequestListeners();

        instanceTracker.GetInstances<TestRequestListener>().ShouldBeEmpty("Listeners should not be created until enumerated");

        var expectedListener = listeners.Last();
        listeners.Last().ShouldBe(expectedListener, "Should be same instance");

        var createdInstances = instanceTracker.GetInstances<TestRequestListener>().Cast<ISlackRequestListener>();
        createdInstances.ShouldMatchReferences(new[] { expectedListener }, "Listeners should be created once on enumeration");
    }

    [Test]
    public void UseHandlerFactoryType()
    {
        UseService<ISlackHandlerFactory, TestHandlerFactory>(
            c => c.UseHandlerFactory<TestHandlerFactory>(),
            s => s.GetHandlerFactory());
    }

    [Test]
    public void UseApiClientType()
    {
        UseService<ISlackApiClient, TestApiClient>(
            c => c.UseApiClient<TestApiClient>(),
            s => s.GetApiClient());
    }

    [Test]
    public void UseSocketModeClientType()
    {
        UseService<ISlackSocketModeClient, TestSocketModeClient>(
            c => c.UseSocketModeClient<TestSocketModeClient>(),
            s => s.GetSocketModeClient());
    }

    [Test]
    public void ReplaceEventHandling_WithType()
    {
        ReplaceRequestHandling<IEventHandler, TestEventHandler>(
            c => c.ReplaceEventHandling<TestEventHandler>(),
            (hf, ctx) => hf.CreateEventHandler(ctx));
    }

    [Test]
    public void ReplaceBlockActionHandling_WithType()
    {
        ReplaceRequestHandling<IAsyncBlockActionHandler, TestAsyncBlockActionHandler>(
            c => c.ReplaceBlockActionHandling<TestAsyncBlockActionHandler>(),
            (hf, ctx) => hf.CreateBlockActionHandler(ctx));
    }

    [Test]
    public void ReplaceBlockOptionProvider_WithType()
    {
        ReplaceRequestHandling<IBlockOptionProvider, TestBlockOptionProvider>(
            c => c.ReplaceBlockOptionProviding<TestBlockOptionProvider>(),
            (hf, ctx) => hf.CreateBlockOptionProvider(ctx));
    }

    [Test]
    public void ReplaceMessageShortcutHandling_WithType()
    {
        ReplaceRequestHandling<IAsyncMessageShortcutHandler, TestAsyncMessageShortcutHandler>(
            c => c.ReplaceMessageShortcutHandling<TestAsyncMessageShortcutHandler>(),
            (hf, ctx) => hf.CreateMessageShortcutHandler(ctx));
    }

    [Test]
    public void ReplaceGlobalShortcutHandling_WithType()
    {
        ReplaceRequestHandling<IAsyncGlobalShortcutHandler, TestAsyncGlobalShortcutHandler>(
            c => c.ReplaceGlobalShortcutHandling<TestAsyncGlobalShortcutHandler>(),
            (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx));
    }

    [Test]
    public void ReplaceViewSubmissionHandling_WithType()
    {
        ReplaceRequestHandling<IAsyncViewSubmissionHandler, TestAsyncViewSubmissionHandler>(
            c => c.ReplaceViewSubmissionHandling<TestAsyncViewSubmissionHandler>(),
            (hf, ctx) => hf.CreateViewSubmissionHandler(ctx));
    }

    [Test]
    public void ReplaceSlashCommandHandling_WithType()
    {
        ReplaceRequestHandling<IAsyncSlashCommandHandler, TestAsyncSlashCommandHandler>(
            c => c.ReplaceSlashCommandHandling<TestAsyncSlashCommandHandler>(),
            (hf, ctx) => hf.CreateSlashCommandHandler(ctx));
    }

    [Test]
    public void ReplaceLegacyInteractiveMessageHandling_WithType()
    {
        ReplaceRequestHandling<IInteractiveMessageHandler, TestInteractiveMessageHandler>(
            c => c.ReplaceLegacyInteractiveMessageHandling<TestInteractiveMessageHandler>(),
            (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx));
    }

    [Test]
    public void ReplaceLegacyOptionProviding_WithType()
    {
        ReplaceRequestHandling<IOptionProvider, TestOptionProvider>(
            c => c.ReplaceLegacyOptionProviding<TestOptionProvider>(),
            (hf, ctx) => hf.CreateLegacyOptionProvider(ctx));
    }

    [Test]
    public void ReplaceLegacyDialogSubmissionHandling_WithType()
    {
        ReplaceRequestHandling<IDialogSubmissionHandler, TestDialogSubmissionHandler>(
            c => c.ReplaceLegacyDialogSubmissionHandling<TestDialogSubmissionHandler>(),
            (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx));
    }

    [Test]
    public void RegisterEventHandlerType()
    {
        RegisterEventHandler(
            registerGenericHandler: c => c.RegisterEventHandler<TestEventHandler>(),
            registerTypedHandler: c => c.RegisterEventHandler<Hello, TestEventHandler<Hello>>());
    }

    protected void RegisterEventHandler(Action<TConfig> registerGenericHandler, Action<TConfig> registerTypedHandler)
    {
        var goodbyeEventCallback = new EventCallback { Event = new Goodbye() };
        var helloEvent1 = new Hello();
        var helloEventCallback1 = new EventCallback { Event = helloEvent1 };
        var helloEvent2 = new Hello();
        var helloEventCallback2 = new EventCallback { Event = helloEvent2 };

        var sut = Configure(c =>
            {
                registerGenericHandler(c);
                registerTypedHandler(c);
            });

        // First request
        HandleEvents(sut, new[] { goodbyeEventCallback, helloEventCallback1 });

        var typedHandler = FirstRequestInstance<TestEventHandler<Hello>>();
        typedHandler.Received().Handle(helloEvent1);

        var genericHandler = FirstRequestInstance<TestEventHandler>();
        genericHandler.Received().Handle(goodbyeEventCallback);
        genericHandler.Received().Handle(helloEventCallback1);

        // Second request
        HandleEvents(sut, new[] { helloEventCallback2 });

        SecondRequestInstance<TestEventHandler<Hello>>().Received().Handle(helloEvent2);
        SecondRequestInstance<TestEventHandler>().Received().Handle(helloEventCallback2);
    }

    [Test]
    public void RegisterBlockActionHandlerType()
    {
        RegisterBlockActionHandler(
            registerKeyedButtonHandler: (c, key) => c.RegisterBlockActionHandler<ButtonAction, TestBlockActionHandler<ButtonAction>>(key),
            registerButtonHandler: c => c.RegisterBlockActionHandler<DatePickerAction, TestBlockActionHandler<DatePickerAction>>(),
            registerGenericHandler: c => c.RegisterBlockActionHandler<TestBlockActionHandler>(),
            registerKeyedAsyncDatePickerHandler: (c, key) => c.RegisterAsyncBlockActionHandler<ButtonAction, TestAsyncBlockActionHandler<ButtonAction>>(key),
            registerAsyncDatePickerHandler: c => c.RegisterAsyncBlockActionHandler<DatePickerAction, TestAsyncBlockActionHandler<DatePickerAction>>(),
            registerGenericAsyncHandler: c => c.RegisterAsyncBlockActionHandler<TestAsyncBlockActionHandler>());
    }

    protected void RegisterBlockActionHandler(
        Action<TConfig, string> registerKeyedButtonHandler,
        Action<TConfig> registerButtonHandler,
        Action<TConfig> registerGenericHandler,
        Action<TConfig, string> registerKeyedAsyncDatePickerHandler,
        Action<TConfig> registerAsyncDatePickerHandler,
        Action<TConfig> registerGenericAsyncHandler)
    {
        var overflowRequest = new BlockActionRequest { Actions = { new OverflowAction { ActionId = "other" } } };
        var buttonAction1 = new ButtonAction { ActionId = "key" };
        var buttonRequest1 = new BlockActionRequest { Actions = { buttonAction1 } };
        var buttonAction2 = new ButtonAction { ActionId = "key" };
        var buttonRequest2 = new BlockActionRequest { Actions = { buttonAction2 } };
        var otherButtonAction = new ButtonAction { ActionId = "other" };
        var otherButtonRequest = new BlockActionRequest { Actions = { otherButtonAction } };
        var datePickerAction1 = new DatePickerAction();
        var datePickerRequest1 = new BlockActionRequest { Actions = { datePickerAction1 } };
        var datePickerAction2 = new DatePickerAction();
        var datePickerRequest2 = new BlockActionRequest { Actions = { datePickerAction2 } };
        var responder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerKeyedButtonHandler(c, "key");
                registerButtonHandler(c);
                registerGenericHandler(c);
                registerKeyedAsyncDatePickerHandler(c, "key");
                registerAsyncDatePickerHandler(c);
                registerGenericAsyncHandler(c);
            });

        // First request
        HandleBlockActions(sut, responder, new[] { overflowRequest, buttonRequest1, otherButtonRequest, datePickerRequest1 });

        var keyedHandler1 = FirstRequestInstance<TestBlockActionHandler<ButtonAction>>();
        keyedHandler1.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest);
        keyedHandler1.DidNotReceive().Handle(otherButtonAction, otherButtonRequest);
        keyedHandler1.Received().Handle(buttonAction1, buttonRequest1);
        keyedHandler1.DidNotReceive().Handle(Arg.Any<ButtonAction>(), datePickerRequest1);

        var typedHandler1 = FirstRequestInstance<TestBlockActionHandler<DatePickerAction>>();
        typedHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), overflowRequest);
        typedHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), otherButtonRequest);
        typedHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), buttonRequest1);
        typedHandler1.Received().Handle(datePickerAction1, datePickerRequest1);

        var genericHandler1 = FirstRequestInstance<TestBlockActionHandler>();
        genericHandler1.Received().Handle(overflowRequest);
        genericHandler1.Received().Handle(otherButtonRequest);
        genericHandler1.Received().Handle(buttonRequest1);
        genericHandler1.Received().Handle(datePickerRequest1);

        var keyedAsyncHandler1 = FirstRequestInstance<TestAsyncBlockActionHandler<ButtonAction>>();
        keyedAsyncHandler1.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest, responder);
        keyedAsyncHandler1.DidNotReceive().Handle(otherButtonAction, otherButtonRequest, responder);
        keyedAsyncHandler1.Received().Handle(buttonAction1, buttonRequest1, responder);
        keyedAsyncHandler1.DidNotReceive().Handle(Arg.Any<ButtonAction>(), datePickerRequest1, responder);

        var typedAsyncHandler1 = FirstRequestInstance<TestAsyncBlockActionHandler<DatePickerAction>>();
        typedAsyncHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), overflowRequest, responder);
        typedAsyncHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), otherButtonRequest, responder);
        typedAsyncHandler1.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), buttonRequest1, responder);
        typedAsyncHandler1.Received().Handle(datePickerAction1, datePickerRequest1, responder);

        var genericAsyncHandler1 = FirstRequestInstance<TestAsyncBlockActionHandler>();
        genericAsyncHandler1.Received().Handle(overflowRequest, responder);
        genericAsyncHandler1.Received().Handle(otherButtonRequest, responder);
        genericAsyncHandler1.Received().Handle(buttonRequest1, responder);
        genericAsyncHandler1.Received().Handle(datePickerRequest1, responder);

        // Second request
        HandleBlockActions(sut, responder, new[] { buttonRequest2, datePickerRequest2 });

        var keyedHandler2 = SecondRequestInstance<TestBlockActionHandler<ButtonAction>>();
        keyedHandler2.Received().Handle(buttonAction2, buttonRequest2);
        keyedHandler2.DidNotReceive().Handle(Arg.Any<ButtonAction>(), datePickerRequest2);

        var typedHandler2 = SecondRequestInstance<TestBlockActionHandler<DatePickerAction>>();
        typedHandler2.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), buttonRequest2);
        typedHandler2.Received().Handle(datePickerAction2, datePickerRequest2);

        var genericHandler2 = SecondRequestInstance<TestBlockActionHandler>();
        genericHandler2.Received().Handle(buttonRequest2);
        genericHandler2.Received().Handle(datePickerRequest2);

        var keyedAsyncHandler2 = SecondRequestInstance<TestAsyncBlockActionHandler<ButtonAction>>();
        keyedAsyncHandler2.Received().Handle(buttonAction2, buttonRequest2, responder);
        keyedAsyncHandler2.DidNotReceive().Handle(Arg.Any<ButtonAction>(), datePickerRequest2, responder);

        var typedAsyncHandler2 = SecondRequestInstance<TestAsyncBlockActionHandler<DatePickerAction>>();
        typedAsyncHandler2.DidNotReceive().Handle(Arg.Any<DatePickerAction>(), buttonRequest2, responder);
        typedAsyncHandler2.Received().Handle(datePickerAction2, datePickerRequest2, responder);

        var genericAsyncHandler2 = SecondRequestInstance<TestAsyncBlockActionHandler>();
        genericAsyncHandler2.Received().Handle(buttonRequest2, responder);
        genericAsyncHandler2.Received().Handle(datePickerRequest2, responder);
    }

    [Test]
    public void RegisterBlockOptionProviderType()
    {
        RegisterBlockOptionProvider(
            registerProvider: (c, action) => c.RegisterBlockOptionProvider<TestBlockOptionProvider>(action));
    }

    protected void RegisterBlockOptionProvider(Action<TConfig, string> registerProvider)
    {
        var otherOptionsRequest = new BlockOptionsRequest { ActionId = "other" };
        var optionsRequest1 = new BlockOptionsRequest { ActionId = "action" };
        var optionsRequest2 = new BlockOptionsRequest { ActionId = "action" };

        var sut = Configure(c => registerProvider(c, "action"));

        // First request
        HandleBlockOptionRequests(sut, new[] { optionsRequest1 });

        var provider = FirstRequestInstance<TestBlockOptionProvider>();
        provider.DidNotReceive().GetOptions(otherOptionsRequest);
        provider.Received().GetOptions(optionsRequest1);

        // Second request
        HandleBlockOptionRequests(sut, new[] { optionsRequest2 });

        SecondRequestInstance<TestBlockOptionProvider>().Received().GetOptions(optionsRequest2);
    }

    [Test]
    public void RegisterMessageShortcutHandlerType()
    {
        RegisterMessageShortcutHandler(
            registerHandler: c => c.RegisterMessageShortcutHandler<TestMessageShortcutHandler>(),
            registerAsyncHandler: c => c.RegisterAsyncMessageShortcutHandler<TestAsyncMessageShortcutHandler>());
    }

    protected void RegisterMessageShortcutHandler(
        Action<TConfig> registerHandler,
        Action<TConfig> registerAsyncHandler)
    {
        var shortcut1 = new MessageShortcut();
        var shortcut2 = new MessageShortcut();
        var responder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerHandler(c);
                registerAsyncHandler(c);
            });

        // First request
        HandleMessageShortcuts(sut, responder, new[] { shortcut1 });

        FirstRequestInstance<TestMessageShortcutHandler>().Received().Handle(shortcut1);
        FirstRequestInstance<TestAsyncMessageShortcutHandler>().Received().Handle(shortcut1, responder);

        // Second request
        HandleMessageShortcuts(sut, responder, new[] { shortcut2 });

        SecondRequestInstance<TestMessageShortcutHandler>().Received().Handle(shortcut2);
        SecondRequestInstance<TestAsyncMessageShortcutHandler>().Received().Handle(shortcut2, responder);
    }

    [Test]
    public void RegisterMessageShortcutHandlerType_Keyed()
    {
        RegisterMessageShortcutHandler_Keyed(
            registerHandler: (c, key) => c.RegisterMessageShortcutHandler<TestMessageShortcutHandler>(key),
            registerAsyncHandler: (c, key) => c.RegisterAsyncMessageShortcutHandler<TestAsyncMessageShortcutHandler>(key));
    }

    protected void RegisterMessageShortcutHandler_Keyed(
        Action<TConfig, string> registerHandler,
        Action<TConfig, string> registerAsyncHandler)
    {
        var otherShortcut = new MessageShortcut { CallbackId = "other" };
        var shortcut1 = new MessageShortcut { CallbackId = "key" };
        var shortcut2 = new MessageShortcut { CallbackId = "key" };
        var responder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerHandler(c, "key");
                registerAsyncHandler(c, "key");
            });

        // First request
        HandleMessageShortcuts(sut, responder, new[] { otherShortcut, shortcut1 });

        var handler = FirstRequestInstance<TestMessageShortcutHandler>();
        handler.DidNotReceive().Handle(otherShortcut);
        handler.Received().Handle(shortcut1);

        var asyncHandler = FirstRequestInstance<TestAsyncMessageShortcutHandler>();
        asyncHandler.DidNotReceive().Handle(otherShortcut, responder);
        asyncHandler.Received().Handle(shortcut1, responder);

        // Second request
        HandleMessageShortcuts(sut, responder, new[] { shortcut2 });

        SecondRequestInstance<TestMessageShortcutHandler>().Received().Handle(shortcut2);
        SecondRequestInstance<TestAsyncMessageShortcutHandler>().Received().Handle(shortcut2, responder);
    }

    [Test]
    public void RegisterGlobalShortcutHandlerType()
    {
        RegisterGlobalShortcutHandler(
            registerHandler: c => c.RegisterGlobalShortcutHandler<TestGlobalShortcutHandler>(),
            registerAsyncHandler: c => c.RegisterAsyncGlobalShortcutHandler<TestAsyncGlobalShortcutHandler>());
    }

    protected void RegisterGlobalShortcutHandler(
        Action<TConfig> registerHandler,
        Action<TConfig> registerAsyncHandler)
    {
        var shortcut1 = new GlobalShortcut();
        var shortcut2 = new GlobalShortcut();
        var responder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerHandler(c);
                registerAsyncHandler(c);
            });

        // First request
        HandleGlobalShortcuts(sut, responder, new[] { shortcut1 });

        FirstRequestInstance<TestGlobalShortcutHandler>().Received().Handle(shortcut1);
        FirstRequestInstance<TestAsyncGlobalShortcutHandler>().Received().Handle(shortcut1, responder);

        // Second request
        HandleGlobalShortcuts(sut, responder, new[] { shortcut2 });

        SecondRequestInstance<TestGlobalShortcutHandler>().Received().Handle(shortcut2);
        SecondRequestInstance<TestAsyncGlobalShortcutHandler>().Received().Handle(shortcut2, responder);
    }

    [Test]
    public void RegisterGlobalShortcutHandlerType_Keyed()
    {
        RegisterGlobalShortcutHandler_Keyed(
            registerHandler: (c, key) => c.RegisterGlobalShortcutHandler<TestGlobalShortcutHandler>(key),
            registerAsyncHandler: (c, key) => c.RegisterAsyncGlobalShortcutHandler<TestAsyncGlobalShortcutHandler>(key));
    }

    protected void RegisterGlobalShortcutHandler_Keyed(
        Action<TConfig, string> registerHandler,
        Action<TConfig, string> registerAsyncHandler)
    {
        var otherShortcut = new GlobalShortcut { CallbackId = "other" };
        var shortcut1 = new GlobalShortcut { CallbackId = "key" };
        var shortcut2 = new GlobalShortcut { CallbackId = "key" };
        var responder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerHandler(c, "key");
                registerAsyncHandler(c, "key");
            });

        // First request
        HandleGlobalShortcuts(sut, responder, new[] { otherShortcut, shortcut1 });

        var handler = FirstRequestInstance<TestGlobalShortcutHandler>();
        handler.DidNotReceive().Handle(otherShortcut);
        handler.Received().Handle(shortcut1);

        var asyncHandler = FirstRequestInstance<TestAsyncGlobalShortcutHandler>();
        asyncHandler.DidNotReceive().Handle(otherShortcut, responder);
        asyncHandler.Received().Handle(shortcut1, responder);

        // Second request
        HandleGlobalShortcuts(sut, responder, new[] { shortcut2 });

        SecondRequestInstance<TestGlobalShortcutHandler>().Received().Handle(shortcut2);
        SecondRequestInstance<TestAsyncGlobalShortcutHandler>().Received().Handle(shortcut2, responder);
    }

    [Test]
    public void RegisterViewSubmissionHandlerType_HandleSubmissions()
    {
        RegisterViewSubmissionHandler_HandleSubmissions(
            registerHandler: (c, key) => c.RegisterViewSubmissionHandler<TestViewSubmissionHandler>(key),
            registerAsyncHandler: (c, key) => c.RegisterAsyncViewSubmissionHandler<TestAsyncViewSubmissionHandler>(key));
    }

    protected void RegisterViewSubmissionHandler_HandleSubmissions(
        Action<TConfig, string> registerHandler,
        Action<TConfig, string> registerAsyncHandler)
    {
        var otherViewSubmission = new ViewSubmission { View = new HomeViewInfo { CallbackId = "other" } };
        var viewSubmission1 = new ViewSubmission { View = new HomeViewInfo { CallbackId = "key" } };
        var viewSubmission2 = new ViewSubmission { View = new HomeViewInfo { CallbackId = "key" } };
        var asyncViewSubmission1 = new ViewSubmission { View = new HomeViewInfo { CallbackId = "asyncKey" } };
        var asyncViewSubmission2 = new ViewSubmission { View = new HomeViewInfo { CallbackId = "asyncKey" } };
        var submissionResponder = Substitute.For<Responder<ViewSubmissionResponse>>();

        var sut = Configure(c =>
            {
                registerHandler(c, "key");
                registerAsyncHandler(c, "asyncKey");
            });

        // First request
        HandleViewSubmissions(sut, submissionResponder, new[] { otherViewSubmission, viewSubmission1, asyncViewSubmission1 });

        var handler = FirstRequestInstance<TestViewSubmissionHandler>();
        handler.DidNotReceive().Handle(otherViewSubmission);
        handler.Received().Handle(viewSubmission1);
        handler.DidNotReceive().Handle(asyncViewSubmission1);
        var asyncHandler = FirstRequestInstance<TestAsyncViewSubmissionHandler>();
        asyncHandler.DidNotReceive().Handle(otherViewSubmission, submissionResponder);
        asyncHandler.DidNotReceive().Handle(viewSubmission1, submissionResponder);
        asyncHandler.Received().Handle(asyncViewSubmission1, submissionResponder);

        // Second request
        HandleViewSubmissions(sut, submissionResponder, new[] { viewSubmission2, asyncViewSubmission2 });

        SecondRequestInstance<TestViewSubmissionHandler>().Received().Handle(viewSubmission2);
        SecondRequestInstance<TestAsyncViewSubmissionHandler>().Received().Handle(asyncViewSubmission2, submissionResponder);
    }

    [Test]
    public void RegisterViewSubmissionHandlerType_HandleCloses()
    {
        RegisterViewSubmissionHandler_HandleCloses(
            registerHandler: (c, key) => c.RegisterViewSubmissionHandler<TestViewSubmissionHandler>(key),
            registerAsyncHandler: (c, key) => c.RegisterAsyncViewSubmissionHandler<TestAsyncViewSubmissionHandler>(key));
    }

    protected void RegisterViewSubmissionHandler_HandleCloses(
        Action<TConfig, string> registerHandler,
        Action<TConfig, string> registerAsyncHandler)
    {
        var otherViewClosed = new ViewClosed { View = new HomeViewInfo { CallbackId = "other" } };
        var viewClosed1 = new ViewClosed { View = new HomeViewInfo { CallbackId = "key" } };
        var viewClosed2 = new ViewClosed { View = new HomeViewInfo { CallbackId = "key" } };
        var asyncViewClosed1 = new ViewClosed { View = new HomeViewInfo { CallbackId = "asyncKey" } };
        var asyncViewClosed2 = new ViewClosed { View = new HomeViewInfo { CallbackId = "asyncKey" } };
        var closeResponder = Substitute.For<Responder>();

        var sut = Configure(c =>
            {
                registerHandler(c, "key");
                registerAsyncHandler(c, "asyncKey");
            });

        // First request
        HandleViewCloses(sut, closeResponder, new[] { otherViewClosed, viewClosed1, asyncViewClosed1 });

        var handler = FirstRequestInstance<TestViewSubmissionHandler>();
        handler.DidNotReceive().HandleClose(otherViewClosed);
        handler.Received().HandleClose(viewClosed1);
        handler.DidNotReceive().HandleClose(asyncViewClosed1);
        var asyncHandler = FirstRequestInstance<TestAsyncViewSubmissionHandler>();
        asyncHandler.DidNotReceive().HandleClose(otherViewClosed, closeResponder);
        asyncHandler.DidNotReceive().HandleClose(viewClosed1, closeResponder);
        asyncHandler.Received().HandleClose(asyncViewClosed1, closeResponder);

        // Second request
        HandleViewCloses(sut, closeResponder, new[] { viewClosed2, asyncViewClosed2 });

        SecondRequestInstance<TestViewSubmissionHandler>().Received().HandleClose(viewClosed2);
        SecondRequestInstance<TestAsyncViewSubmissionHandler>().Received().HandleClose(asyncViewClosed2, closeResponder);
    }

    [Test]
    public void RegisterSlashCommandHandlerType_InvalidCommandName_Throws()
    {
        RegisterSlashCommandHandlerWithInvalidCommandName(
            c => c.RegisterSlashCommandHandler<TestSlashCommandHandler>("foo"),
            c => c.RegisterAsyncSlashCommandHandler<TestAsyncSlashCommandHandler>("foo"));
    }

    [Test]
    public void RegisterSlashCommandHandlerType()
    {
        RegisterSlashCommandHandler(
            registerHandler: (c, command) => c.RegisterSlashCommandHandler<TestSlashCommandHandler>(command),
            registerAsyncHandler: (c, command) => c.RegisterAsyncSlashCommandHandler<TestAsyncSlashCommandHandler>(command));
    }

    protected void RegisterSlashCommandHandler(
        Action<TConfig, string> registerHandler,
        Action<TConfig, string> registerAsyncHandler)
    {
        var otherSlashCommand = new SlashCommand { Command = "/other" };
        var slashCommand1 = new SlashCommand { Command = "/command" };
        var slashCommand2 = new SlashCommand { Command = "/command" };
        var asyncSlashCommand1 = new SlashCommand { Command = "/asyncCommand" };
        var asyncSlashCommand2 = new SlashCommand { Command = "/asyncCommand" };
        var responder = Substitute.For<Responder<SlashCommandResponse>>();

        var sut = Configure(c =>
            {
                registerHandler(c, "/command");
                registerAsyncHandler(c, "/asyncCommand");
            });

        // First request
        HandleSlashCommands(sut, responder, new[] { otherSlashCommand, slashCommand1, asyncSlashCommand1 });

        var handler = FirstRequestInstance<TestSlashCommandHandler>();
        handler.DidNotReceive().Handle(otherSlashCommand);
        handler.Received().Handle(slashCommand1);
        handler.DidNotReceive().Handle(asyncSlashCommand1);
        var asyncHandler = FirstRequestInstance<TestAsyncSlashCommandHandler>();
        asyncHandler.DidNotReceive().Handle(otherSlashCommand, responder);
        asyncHandler.DidNotReceive().Handle(slashCommand1, responder);
        asyncHandler.Received().Handle(asyncSlashCommand1, responder);

        // Second request
        HandleSlashCommands(sut, responder, new[] { slashCommand2, asyncSlashCommand2 });

        SecondRequestInstance<TestSlashCommandHandler>().Received().Handle(slashCommand2);
        SecondRequestInstance<TestAsyncSlashCommandHandler>().Received().Handle(asyncSlashCommand2, responder);
    }

    [Test]
    public void RegisterInteractiveMessageHandlerType()
    {
        RegisterInteractiveMessageHandler(
            registerHandler: (c, action) => c.RegisterInteractiveMessageHandler<TestInteractiveMessageHandler>(action));
    }

    protected void RegisterInteractiveMessageHandler(Action<TConfig, string> registerHandler)
    {
        var otherInteractiveMessage = new InteractiveMessage { Actions = { new Interaction.Button { Name = "other" } } };
        var interactiveMessage1 = new InteractiveMessage { Actions = { new Interaction.Button { Name = "action" } } };
        var interactiveMessage2 = new InteractiveMessage { Actions = { new Interaction.Button { Name = "action" } } };

        var sut = Configure(c => registerHandler(c, "action"));

        // First request
        HandleLegacyInteractiveMessages(sut, new[] { otherInteractiveMessage, interactiveMessage1 });

        var handler = FirstRequestInstance<TestInteractiveMessageHandler>();
        handler.DidNotReceive().Handle(otherInteractiveMessage);
        handler.Received().Handle(interactiveMessage1);

        // Second request
        HandleLegacyInteractiveMessages(sut, new[] { interactiveMessage2 });

        SecondRequestInstance<TestInteractiveMessageHandler>().Received().Handle(interactiveMessage2);
    }

    [Test]
    public void RegisterOptionProviderType()
    {
        RegisterOptionProvider(
            registerHandler: (c, action) => c.RegisterOptionProvider<TestOptionProvider>(action));
    }

    protected void RegisterOptionProvider(Action<TConfig, string> registerHandler)
    {
        var otherOptionsRequest = new OptionsRequest { Name = "other" };
        var optionsRequest1 = new OptionsRequest { Name = "action" };
        var optionsRequest2 = new OptionsRequest { Name = "action" };

        var sut = Configure(c => registerHandler(c, "action"));

        // First request
        HandleLegacyOptionsRequest(sut, new[] { otherOptionsRequest, optionsRequest1 });

        var handler = FirstRequestInstance<TestOptionProvider>();
        handler.DidNotReceive().GetOptions(otherOptionsRequest);
        handler.Received().GetOptions(optionsRequest1);

        // Second request
        HandleLegacyOptionsRequest(sut, new[] { optionsRequest2 });

        SecondRequestInstance<TestOptionProvider>().Received().GetOptions(optionsRequest2);
    }

    [Test]
    public void RegisterDialogSubmissionHandlerType_HandleSubmissions()
    {
        RegisterDialogSubmissionHandler_HandleSubmissions(
            registerHandler: (c, key) => c.RegisterDialogSubmissionHandler<TestDialogSubmissionHandler>(key));
    }

    protected void RegisterDialogSubmissionHandler_HandleSubmissions(Action<TConfig, string> registerHandler)
    {
        var otherDialogSubmission = new DialogSubmission { CallbackId = "other" };
        var dialogSubmission1 = new DialogSubmission { CallbackId = "key" };
        var dialogSubmission2 = new DialogSubmission { CallbackId = "key" };

        var sut = Configure(c => registerHandler(c, "key"));

        // First request
        HandleLegacyDialogSubmissions(sut, new[] { otherDialogSubmission, dialogSubmission1 });

        var handler = FirstRequestInstance<TestDialogSubmissionHandler>();
        handler.DidNotReceive().Handle(otherDialogSubmission);
        handler.Received().Handle(dialogSubmission1);

        // Second request
        HandleLegacyDialogSubmissions(sut, new[] { dialogSubmission2 });

        SecondRequestInstance<TestDialogSubmissionHandler>().Received().Handle(dialogSubmission2);
    }

    [Test]
    public void RegisterDialogSubmissionHandlerType_HandleCancellations()
    {
        RegisterDialogSubmissionHandler_HandleCancellations(
            registerHandler: (c, key) => c.RegisterDialogSubmissionHandler<TestDialogSubmissionHandler>(key));
    }

    protected void RegisterDialogSubmissionHandler_HandleCancellations(Action<TConfig, string> registerHandler)
    {
        var otherDialogCancel = new DialogCancellation { CallbackId = "other" };
        var dialogCancel1 = new DialogCancellation { CallbackId = "key" };
        var dialogCancel2 = new DialogCancellation { CallbackId = "key" };

        var sut = Configure(c => registerHandler(c, "key"));

        // First request
        HandleLegacyDialogCancellations(sut, new[] { otherDialogCancel, dialogCancel1 });

        var handler = FirstRequestInstance<TestDialogSubmissionHandler>();
        handler.DidNotReceive().HandleCancel(otherDialogCancel);
        handler.Received().HandleCancel(dialogCancel1);

        // Second request
        HandleLegacyDialogCancellations(sut, new[] { dialogCancel2 });

        SecondRequestInstance<TestDialogSubmissionHandler>().Received().HandleCancel(dialogCancel2);
    }

    protected void UseService<TService, TImplementation>(Action<TConfig> registerService, Func<ISlackServiceProvider, TService> getService) where TService : class where TImplementation : TService
    {
        var sut = Configure(registerService);

        var service = getService(sut);
        service.ShouldReferTo(getService(sut), "Should be same instance")
            .And.ShouldBeA<TImplementation>();

        DuringRequest(sut, _ => getService(sut).ShouldReferTo(service, "Should be same instance during request"));
    }

    protected void ReplaceRequestHandling<THandler, TImplementation>(
        Action<TConfig> replaceHandling,
        Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
        where THandler : class
        where TImplementation : class, THandler
    {
        var sut = Configure(replaceHandling);

        THandler firstRequestHandler = default;

        DuringRequest(sut,
            c =>
                {
                    firstRequestHandler = getHandler(sut.GetHandlerFactory(), c);
                    firstRequestHandler.ShouldBeA<TImplementation>();
                });

        DuringRequest(sut,
            c =>
                {
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldNotReferTo(firstRequestHandler, DifferentInstanceForDifferentRequest)
                        .And.ShouldBeA<TImplementation>();
                });
    }

    private THandler FirstRequestInstance<THandler>() where THandler : TrackedClass =>
        InstanceTracker.GetInstances<THandler>().ShouldBeSingular(SameInstanceForSameRequest).And.Single();

    private THandler SecondRequestInstance<THandler>() where THandler : TrackedClass =>
        InstanceTracker.GetInstances<THandler>().ShouldBeLength(2, DifferentInstanceForDifferentRequest).And.Last();

    protected class TestHttp : IHttp
    {
        public Task<T> Execute<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }

    protected class TestJsonSettings() : SlackJsonSettings(new JsonSerializerSettings());

    protected class TestTypeResolver : ISlackTypeResolver
    {
        public Type FindType(Type baseType, string slackType) => throw new NotImplementedException();
    }

    protected class TestUrlBuilder : ISlackUrlBuilder
    {
        public string Url(string apiMethod, Dictionary<string, object> args) => throw new NotImplementedException();
    }

    protected class TestLogger : ILogger
    {
        public void Log(ILogEvent logEvent) => throw new NotImplementedException();
    }

    protected class TestWebSocketFactory : IWebSocketFactory
    {
        public IWebSocket Create(string uri) => throw new NotImplementedException();
    }

    protected class TestHandlerFactory : ISlackHandlerFactory
    {
        public IEventHandler CreateEventHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IAsyncBlockActionHandler CreateBlockActionHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IBlockOptionProvider CreateBlockOptionProvider(SlackRequestContext context) => throw new NotImplementedException();
        public IAsyncMessageShortcutHandler CreateMessageShortcutHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IAsyncGlobalShortcutHandler CreateGlobalShortcutHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IAsyncViewSubmissionHandler CreateViewSubmissionHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IAsyncSlashCommandHandler CreateSlashCommandHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IInteractiveMessageHandler CreateLegacyInteractiveMessageHandler(SlackRequestContext context) => throw new NotImplementedException();
        public IOptionProvider CreateLegacyOptionProvider(SlackRequestContext context) => throw new NotImplementedException();
        public IDialogSubmissionHandler CreateLegacyDialogSubmissionHandler(SlackRequestContext context) => throw new NotImplementedException();
    }

    protected class TestApiClient : ISlackApiClient
    {
        public IApiApi Api { get; }
        public IAppsConnectionsApi AppsConnectionsApi { get; }
        public IAppsEventAuthorizationsApi AppsEventAuthorizations { get; }
        public IAssistantSearchApi AssistantSearch { get; }
        public IAssistantThreadsApi AssistantThreads { get; }
        public IAuthApi Auth { get; }
        public IBookmarksApi Bookmarks { get; }
        public IBotsApi Bots { get; }
        public ICallParticipantsApi CallParticipants { get; }
        public ICallsApi Calls { get; }
        public ICanvasesApi Canvases { get; }
        public IChatApi Chat { get; }
        public IConversationsApi Conversations { get; }
        public IDialogApi Dialog { get; }
        public IDndApi Dnd { get; }
        public IEmojiApi Emoji { get; }
        public IEntityApi Entity { get; }
        public IExternalTeamsApi ExternalTeams { get; }
        public IFileCommentsApi FileComments { get; }
        public IFilesApi Files { get; }
        public IListApi List { get; }
        public IListDownloadApi ListDownload { get; }
        public IListItemsApi ListItems { get; }
        public IListAccessApi ListAccess { get; }
        public IMigrationApi Migration { get; }
        public IOAuthApi OAuth { get; }
        public IOAuthV2Api OAuthV2 { get; }
        public IOpenIdApi OpenIdApi { get; }
        public IPinsApi Pins { get; }
        public IReactionsApi Reactions { get; }
        public IRemindersApi Reminders { get; }
        public IRemoteFilesApi RemoteFiles { get; }
        public IRtmApi Rtm { get; }
        public IScheduledMessagesApi ScheduledMessages { get; }
        public ISearchApi Search { get; }
        public ITeamApi Team { get; }
        public ITeamBillingApi TeamBilling { get; }
        public ITeamPreferencesApi TeamPreferences { get; }
        public ITeamProfileApi TeamProfile { get; }
        public IToolingApi Tooling { get; }
        public IUserGroupsApi UserGroups { get; }
        public IUserGroupUsersApi UserGroupUsers { get; }
        public IUsersApi Users { get; }
        public IUserProfileApi UserProfile { get; }
        public IViewsApi Views { get; }
        public Task Get(string apiMethod, Dictionary<string, object> args, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<T> Get<T>(string apiMethod, Dictionary<string, object> args, CancellationToken cancellationToken) where T : class => throw new NotImplementedException();
        public Task Post(string apiMethod, Dictionary<string, object> args, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<T> Post<T>(string apiMethod, Dictionary<string, object> args, CancellationToken cancellationToken) where T : class => throw new NotImplementedException();
        public Task Post(string apiMethod, Dictionary<string, object> args, HttpContent content, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<T> Post<T>(string apiMethod, Dictionary<string, object> args, HttpContent content, CancellationToken cancellationToken) where T : class => throw new NotImplementedException();
        public Task Respond(string responseUrl, IReadOnlyMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task PostToWebhook(string webhookUrl, Message message, CancellationToken cancellationToken) => throw new NotImplementedException();
        public ISlackApiClient WithAccessToken(string accessToken) => throw new NotImplementedException();
    }

    protected class TestSocketModeClient : ISlackSocketModeClient
    {
        public void Dispose() => throw new NotImplementedException();
        public Task Connect(SocketModeConnectionOptions connectionOptions = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public void Disconnect() { throw new NotImplementedException(); }
        public bool Connected { get; }
    }

    protected class TestHandler<THandler>(InstanceTracker tracker) : TrackedClass(tracker)
        where THandler : class
    {
        protected readonly THandler Inner = Substitute.For<THandler>();
        public THandler Received() => Inner.Received();
        public THandler DidNotReceive() => Inner.DidNotReceive();
    }

    protected class TestEventHandler(InstanceTracker tracker) : TestHandler<IEventHandler>(tracker), IEventHandler
    {
        public Task Handle(EventCallback eventCallback) => Inner.Handle(eventCallback);
    }

    protected class TestEventHandler<TEvent>(InstanceTracker tracker) : TestHandler<IEventHandler<TEvent>>(tracker), IEventHandler<TEvent>
        where TEvent : Event
    {
        public Task Handle(TEvent slackEvent) => Inner.Handle(slackEvent);
    }

    protected class TestBlockActionHandler(InstanceTracker tracker) : TestHandler<IBlockActionHandler>(tracker), IBlockActionHandler
    {
        public Task Handle(BlockActionRequest request) => Inner.Handle(request);
    }

    protected class TestBlockActionHandler<TAction>(InstanceTracker tracker) : TestHandler<IBlockActionHandler<TAction>>(tracker), IBlockActionHandler<TAction>
        where TAction : BlockAction
    {
        public Task Handle(TAction action, BlockActionRequest request) => Inner.Handle(action, request);
    }

    protected class TestAsyncBlockActionHandler(InstanceTracker tracker) : TestHandler<IAsyncBlockActionHandler>(tracker), IAsyncBlockActionHandler
    {
        public Task Handle(BlockActionRequest request, Responder respond) => Inner.Handle(request, respond);
    }

    protected class TestAsyncBlockActionHandler<TAction>(InstanceTracker tracker) : TestHandler<IAsyncBlockActionHandler<TAction>>(tracker), IAsyncBlockActionHandler<TAction>
        where TAction : BlockAction
    {
        public Task Handle(TAction action, BlockActionRequest request, Responder respond) => Inner.Handle(action, request, respond);
    }

    protected class TestBlockOptionProvider(InstanceTracker tracker) : TestHandler<IBlockOptionProvider>(tracker), IBlockOptionProvider
    {
        public Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) => Inner.GetOptions(request);
    }

    protected class TestMessageShortcutHandler(InstanceTracker tracker) : TestHandler<IMessageShortcutHandler>(tracker), IMessageShortcutHandler
    {
        public Task Handle(MessageShortcut request) => Inner.Handle(request);
    }

    protected class TestAsyncMessageShortcutHandler(InstanceTracker tracker) : TestHandler<IAsyncMessageShortcutHandler>(tracker), IAsyncMessageShortcutHandler
    {
        public Task Handle(MessageShortcut request, Responder respond) => Inner.Handle(request, respond);
    }

    protected class TestGlobalShortcutHandler(InstanceTracker tracker) : TestHandler<IGlobalShortcutHandler>(tracker), IGlobalShortcutHandler
    {
        public Task Handle(GlobalShortcut request) => Inner.Handle(request);
    }

    protected class TestAsyncGlobalShortcutHandler(InstanceTracker tracker) : TestHandler<IAsyncGlobalShortcutHandler>(tracker), IAsyncGlobalShortcutHandler
    {
        public Task Handle(GlobalShortcut request, Responder respond) => Inner.Handle(request, respond);
    }

    protected class TestViewSubmissionHandler(InstanceTracker tracker) : TestHandler<IViewSubmissionHandler>(tracker), IViewSubmissionHandler
    {
        public Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission) => Inner.Handle(viewSubmission);
        public Task HandleClose(ViewClosed viewClosed) => Inner.HandleClose(viewClosed);
    }

    protected class TestAsyncViewSubmissionHandler(InstanceTracker tracker) : TestHandler<IAsyncViewSubmissionHandler>(tracker), IAsyncViewSubmissionHandler
    {
        public Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond) => Inner.Handle(viewSubmission, respond);
        public Task HandleClose(ViewClosed viewClosed, Responder respond) => Inner.HandleClose(viewClosed, respond);
    }

    protected class TestSlashCommandHandler(InstanceTracker tracker) : TestHandler<ISlashCommandHandler>(tracker), ISlashCommandHandler
    {
        public Task<SlashCommandResponse> Handle(SlashCommand command) => Inner.Handle(command);
    }

    protected class TestAsyncSlashCommandHandler(InstanceTracker tracker) : TestHandler<IAsyncSlashCommandHandler>(tracker), IAsyncSlashCommandHandler
    {
        public Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) => Inner.Handle(command, respond);
    }

    protected class TestOptionProvider(InstanceTracker tracker) : TestHandler<IOptionProvider>(tracker), IOptionProvider
    {
        public Task<OptionsResponse> GetOptions(OptionsRequest request) => Inner.GetOptions(request);
    }

    protected class TestInteractiveMessageHandler(InstanceTracker tracker) : TestHandler<IInteractiveMessageHandler>(tracker), IInteractiveMessageHandler
    {
        public Task<MessageResponse> Handle(InteractiveMessage message) => Inner.Handle(message);
    }

    protected class TestDialogSubmissionHandler(InstanceTracker tracker) : TestHandler<IDialogSubmissionHandler>(tracker), IDialogSubmissionHandler
    {
        public Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) => Inner.Handle(dialog);
        public Task HandleCancel(DialogCancellation cancellation) => Inner.HandleCancel(cancellation);
    }
}