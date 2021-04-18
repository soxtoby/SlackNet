using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Tests.Configuration
{
    public abstract class SlackServiceConfigurationBaseTests<TConfig> where TConfig : SlackServiceConfigurationBase<TConfig>
    {
        [TestCaseSource(nameof(SlackServiceProviderMethods))]
        public void DefaultServices(MethodInfo providerMethod)
        {
            var sut = Configure(_ => { });

            ShouldBeSingleInstance(sut, sp => providerMethod.Invoke(sp, new object[0]), providerMethod.Invoke(sut, new object[0]));
        }

        protected static IEnumerable<TestCaseData> SlackServiceProviderMethods =>
            typeof(ISlackServiceProvider).GetMethods()
                .Select(m => new TestCaseData(m).SetName(m.ReturnType.Name));

        [Test]
        public void UseHttp()
        {
            UseService(
                Substitute.For<IHttp>(),
                (c, sp) => c.UseHttp(sp),
                s => s.GetHttp());
        }

        [Test]
        public void UseJsonSettings()
        {
            UseService(
                new SlackJsonSettings(new JsonSerializerSettings()),
                (c, sp) => c.UseJsonSettings(sp),
                s => s.GetJsonSettings());
        }

        [Test]
        public void UseTypeResolver()
        {
            UseService(
                Substitute.For<ISlackTypeResolver>(),
                (c, sp) => c.UseTypeResolver(sp),
                s => s.GetTypeResolver());
        }

        [Test]
        public void UseUrlBuilder()
        {
            UseService(
                Substitute.For<ISlackUrlBuilder>(),
                (c, sp) => c.UseUrlBuilder(sp),
                s => s.GetUrlBuilder());
        }

        [Test]
        public void UseWebSocketFactory()
        {
            UseService(
                Substitute.For<IWebSocketFactory>(),
                (c, sp) => c.UseWebSocketFactory(sp),
                s => s.GetWebSocketFactory());
        }

        [Test]
        public void UseRequestContextFactory()
        {
            var requestContextFactory = Substitute.For<ISlackRequestContextFactory>();
            requestContextFactory.CreateRequestContext().Returns(_ => new SlackRequestContext());

            UseService(
                requestContextFactory,
                (c, sp) => c.UseRequestContextFactory(sp),
                s => s.GetRequestContextFactory());
        }

        [Test]
        public void UseRequestListener()
        {
            UseService(
                Substitute.For<ISlackRequestListener>(),
                (c, sp) => c.UseRequestListener(sp),
                s => s.GetRequestListener());
        }

        [Test]
        public void UseHandlerFactory()
        {
            UseService(
                Substitute.For<ISlackHandlerFactory>(),
                (c, sp) => c.UseHandlerFactory(sp),
                s => s.GetHandlerFactory());
        }

        [Test]
        public void UseApiClient()
        {
            UseService(
                Substitute.For<ISlackApiClient>(),
                (c, sp) => c.UseApiClient(sp),
                s => s.GetApiClient());
        }

        [Test]
        public void UseSocketModeClient()
        {
            UseService(
                Substitute.For<ISlackSocketModeClient>(),
                (c, sp) => c.UseSocketModeClient(sp),
                s => s.GetSocketModeClient());
        }

        [Test]
        public void ReplaceEventHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceEventHandling(sp),
                (hf, ctx) => hf.CreateEventHandler(ctx));
        }

        [Test]
        public void ReplaceBlockActionHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceBlockActionHandling(sp),
                (hf, ctx) => hf.CreateBlockActionHandler(ctx));
        }

        [Test]
        public void ReplaceBlockOptionProvider()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceBlockOptionProviding(sp),
                (hf, ctx) => hf.CreateBlockOptionProvider(ctx));
        }

        [Test]
        public void ReplaceMessageShortcutHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceMessageShortcutHandling(sp),
                (hf, ctx) => hf.CreateMessageShortcutHandler(ctx));
        }

        [Test]
        public void ReplaceGlobalShortcutHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceGlobalShortcutHandling(sp),
                (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx));
        }

        [Test]
        public void ReplaceViewSubmissionHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceViewSubmissionHandling(sp),
                (hf, ctx) => hf.CreateViewSubmissionHandler(ctx));
        }

        [Test]
        public void ReplaceSlashCommandHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceSlashCommandHandling(sp),
                (hf, ctx) => hf.CreateSlashCommandHandler(ctx));
        }

        [Test]
        public void ReplaceWorkflowStepEditHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceWorkflowStepEditHandling(sp),
                (hf, ctx) => hf.CreateWorkflowStepEditHandler(ctx));
        }

        [Test]
        public void ReplaceLegacyInteractiveMessageHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceLegacyInteractiveMessageHandling(sp),
                (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx));
        }

        [Test]
        public void ReplaceLegacyOptionProviding()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceLegacyOptionProviding(sp),
                (hf, ctx) => hf.CreateLegacyOptionProvider(ctx));
        }

        [Test]
        public void ReplaceLegacyDialogSubmissionHandling()
        {
            ReplaceRequestHandling(
                (c, sp) => c.ReplaceLegacyDialogSubmissionHandling(sp),
                (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx));
        }

        [Test]
        public void RegisterEventHandler()
        {
            // Arrange
            var genericHandler = Substitute.For<IEventHandler>();
            var helloHandler = Substitute.For<IEventHandler<Hello>>();

            var goodbyeEventCallback = new EventCallback { Event = new Goodbye() };
            var helloEvent = new Hello();
            var helloEventCallback = new EventCallback { Event = helloEvent };

            var sut = Configure(c => c
                .RegisterEventHandler(genericHandler)
                .RegisterEventHandler(helloHandler));

            // Act
            HandleEvents(sut, new[] { goodbyeEventCallback, helloEventCallback });

            // Assert
            genericHandler.Received().Handle(goodbyeEventCallback);
            genericHandler.Received().Handle(helloEventCallback);

            helloHandler.Received(1).Handle(helloEvent);
        }

        [Test]
        public void RegisterBlockActionHandler()
        {
            // Arrange
            var keyedHandler = Substitute.For<IBlockActionHandler<ButtonAction>>();
            var typedHandler = Substitute.For<IBlockActionHandler<ButtonAction>>();
            var genericHandler = Substitute.For<IBlockActionHandler>();
            var keyedAsyncHandler = Substitute.For<IAsyncBlockActionHandler<ButtonAction>>();
            var typedAsyncHandler = Substitute.For<IAsyncBlockActionHandler<ButtonAction>>();
            var genericAsyncHandler = Substitute.For<IAsyncBlockActionHandler>();
            var overflowRequest = new BlockActionRequest { Actions = { new OverflowAction { ActionId = "other" } } };
            var otherButtonAction = new ButtonAction { ActionId = "other" };
            var otherButtonRequest = new BlockActionRequest { Actions = { otherButtonAction } };
            var buttonAction = new ButtonAction { ActionId = "key" };
            var buttonRequest = new BlockActionRequest { Actions = { buttonAction } };
            var responder = Substitute.For<Responder>();

            var sut = Configure(c => c
                .RegisterBlockActionHandler("key", keyedHandler)
                .RegisterBlockActionHandler(typedHandler)
                .RegisterBlockActionHandler(genericHandler)
                .RegisterAsyncBlockActionHandler("key", keyedAsyncHandler)
                .RegisterAsyncBlockActionHandler(typedAsyncHandler)
                .RegisterAsyncBlockActionHandler(genericAsyncHandler));

            // Act
            HandleBlockActions(sut, responder, new[] { overflowRequest, otherButtonRequest, buttonRequest });

            // Assert
            keyedHandler.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest);
            keyedHandler.DidNotReceive().Handle(otherButtonAction, otherButtonRequest);
            keyedHandler.Received().Handle(buttonAction, buttonRequest);

            typedHandler.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest);
            typedHandler.Received().Handle(otherButtonAction, otherButtonRequest);
            typedHandler.Received().Handle(buttonAction, buttonRequest);

            genericHandler.Received().Handle(overflowRequest);
            genericHandler.Received().Handle(otherButtonRequest);
            genericHandler.Received().Handle(buttonRequest);

            keyedAsyncHandler.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest, responder);
            keyedAsyncHandler.DidNotReceive().Handle(otherButtonAction, otherButtonRequest, responder);
            keyedAsyncHandler.Received().Handle(buttonAction, buttonRequest, responder);

            typedAsyncHandler.DidNotReceive().Handle(Arg.Any<ButtonAction>(), overflowRequest, responder);
            typedAsyncHandler.Received().Handle(otherButtonAction, otherButtonRequest, responder);
            typedAsyncHandler.Received().Handle(buttonAction, buttonRequest, responder);

            genericAsyncHandler.Received().Handle(overflowRequest, responder);
            genericAsyncHandler.Received().Handle(otherButtonRequest, responder);
            genericAsyncHandler.Received().Handle(buttonRequest, responder);
        }

        [Test]
        public void RegisterBlockOptionProvider()
        {
            // Arrange
            var handler = Substitute.For<IBlockOptionProvider>();
            var otherOptionsRequest = new BlockOptionsRequest { ActionId = "other" };
            var optionsRequest = new BlockOptionsRequest { ActionId = "action" };

            var sut = Configure(c => c.RegisterBlockOptionProvider("action", handler));

            // Act
            HandleBlockOptionRequests(sut, new[] { otherOptionsRequest, optionsRequest });

            // Assert
            handler.DidNotReceive().GetOptions(otherOptionsRequest);
            handler.Received().GetOptions(optionsRequest);
        }

        [Test]
        public void RegisterMessageShortcutHandler()
        {
            // Arrange
            var keyedHandler = Substitute.For<IMessageShortcutHandler>();
            var genericHandler = Substitute.For<IMessageShortcutHandler>();
            var keyedAsyncHandler = Substitute.For<IAsyncMessageShortcutHandler>();
            var genericAsyncHandler = Substitute.For<IAsyncMessageShortcutHandler>();
            var otherShortcut = new MessageShortcut { CallbackId = "other" };
            var shortcut = new MessageShortcut { CallbackId = "key" };
            var responder = Substitute.For<Responder>();

            var sut = Configure(c => c
                .RegisterMessageShortcutHandler("key", keyedHandler)
                .RegisterMessageShortcutHandler(genericHandler)
                .RegisterAsyncMessageShortcutHandler("key", keyedAsyncHandler)
                .RegisterAsyncMessageShortcutHandler(genericAsyncHandler));

            // Act
            HandleMessageShortcuts(sut, responder, new[] { otherShortcut, shortcut });

            // Assert
            keyedHandler.DidNotReceive().Handle(otherShortcut);
            keyedHandler.Received().Handle(shortcut);

            genericHandler.Received().Handle(otherShortcut);
            genericHandler.Received().Handle(shortcut);

            keyedAsyncHandler.DidNotReceive().Handle(otherShortcut, responder);
            keyedAsyncHandler.Received().Handle(shortcut, responder);

            genericAsyncHandler.Received().Handle(otherShortcut, responder);
            genericAsyncHandler.Received().Handle(otherShortcut, responder);
        }

        [Test]
        public void RegisterGlobalShortcutHandler()
        {
            // Arrange
            var keyedHandler = Substitute.For<IGlobalShortcutHandler>();
            var genericHandler = Substitute.For<IGlobalShortcutHandler>();
            var keyedAsyncHandler = Substitute.For<IAsyncGlobalShortcutHandler>();
            var genericAsyncHandler = Substitute.For<IAsyncGlobalShortcutHandler>();
            var otherShortcut = new GlobalShortcut { CallbackId = "other" };
            var shortcut = new GlobalShortcut { CallbackId = "key" };
            var responder = Substitute.For<Responder>();

            var sut = Configure(c => c
                .RegisterGlobalShortcutHandler("key", keyedHandler)
                .RegisterGlobalShortcutHandler(genericHandler)
                .RegisterAsyncGlobalShortcutHandler("key", keyedAsyncHandler)
                .RegisterAsyncGlobalShortcutHandler(genericAsyncHandler));

            // Act
            HandleGlobalShortcuts(sut, responder, new[] { otherShortcut, shortcut });

            // Assert
            keyedHandler.DidNotReceive().Handle(otherShortcut);
            keyedHandler.Received().Handle(shortcut);

            genericHandler.Received().Handle(otherShortcut);
            genericHandler.Received().Handle(shortcut);

            keyedAsyncHandler.DidNotReceive().Handle(otherShortcut, responder);
            keyedAsyncHandler.Received().Handle(shortcut, responder);

            genericAsyncHandler.Received().Handle(otherShortcut, responder);
            genericAsyncHandler.Received().Handle(otherShortcut, responder);
        }

        [Test]
        public void RegisterViewSubmissionHandler_HandleSubmissions()
        {
            // Arrange
            var handler = Substitute.For<IViewSubmissionHandler>();
            var asyncHandler = Substitute.For<IAsyncViewSubmissionHandler>();
            var otherViewSubmission = new ViewSubmission { View = new HomeViewInfo { CallbackId = "other" } };
            var viewSubmission = new ViewSubmission { View = new HomeViewInfo { CallbackId = "key" } };
            var asyncViewSubmission = new ViewSubmission { View = new HomeViewInfo { CallbackId = "asyncKey" } };
            var submissionResponder = Substitute.For<Responder<ViewSubmissionResponse>>();

            var sut = Configure(c => c
                .RegisterViewSubmissionHandler("key", handler)
                .RegisterAsyncViewSubmissionHandler("asyncKey", asyncHandler));

            // Act
            HandleViewSubmissions(sut, submissionResponder, new[] { otherViewSubmission, viewSubmission, asyncViewSubmission });

            // Assert
            handler.DidNotReceive().Handle(otherViewSubmission);
            handler.Received().Handle(viewSubmission);
            handler.DidNotReceive().Handle(asyncViewSubmission);

            asyncHandler.DidNotReceive().Handle(otherViewSubmission, submissionResponder);
            asyncHandler.DidNotReceive().Handle(viewSubmission, submissionResponder);
            asyncHandler.Received().Handle(asyncViewSubmission, submissionResponder);
        }

        [Test]
        public void RegisterViewSubmissionHandler_HandleCloses()
        {
            // Arrange
            var handler = Substitute.For<IViewSubmissionHandler>();
            var asyncHandler = Substitute.For<IAsyncViewSubmissionHandler>();
            var otherViewClosed = new ViewClosed { View = new HomeViewInfo { CallbackId = "other" } };
            var viewClosed = new ViewClosed { View = new HomeViewInfo { CallbackId = "key" } };
            var asyncViewClosed = new ViewClosed { View = new HomeViewInfo { CallbackId = "asyncKey" } };
            var closeResponder = Substitute.For<Responder>();

            var sut = Configure(c => c
                .RegisterViewSubmissionHandler("key", handler)
                .RegisterAsyncViewSubmissionHandler("asyncKey", asyncHandler));

            // Act
            HandleViewCloses(sut, closeResponder, new[] { otherViewClosed, viewClosed, asyncViewClosed });

            // Assert
            handler.DidNotReceive().HandleClose(otherViewClosed);
            handler.Received().HandleClose(viewClosed);
            handler.DidNotReceive().HandleClose(asyncViewClosed);

            asyncHandler.DidNotReceive().HandleClose(otherViewClosed, closeResponder);
            asyncHandler.DidNotReceive().HandleClose(viewClosed, closeResponder);
            asyncHandler.Received().HandleClose(asyncViewClosed, closeResponder);
        }

        [Test]
        public void RegisterSlashCommandHandler_InvalidCommandName_Throws()
        {
            RegisterSlashCommandHandlerWithInvalidCommandName(
                c => c.RegisterSlashCommandHandler("foo", Substitute.For<ISlashCommandHandler>()),
                c => c.RegisterAsyncSlashCommandHandler("foo", Substitute.For<IAsyncSlashCommandHandler>()));
        }

        protected void RegisterSlashCommandHandlerWithInvalidCommandName(Action<TConfig> register, Action<TConfig> registerAsync)
        {
            Should.Throw<ArgumentException>(() => Configure(register)).And.ParamName.ShouldBe("command");
            Should.Throw<ArgumentException>(() => Configure(registerAsync)).And.ParamName.ShouldBe("command");
        }

        [Test]
        public void RegisterSlashCommandHandler()
        {
            // Arrange
            var handler = Substitute.For<ISlashCommandHandler>();
            var asyncHandler = Substitute.For<IAsyncSlashCommandHandler>();
            var otherSlashCommand = new SlashCommand { Command = "/other" };
            var slashCommand = new SlashCommand { Command = "/command" };
            var asyncSlashCommand = new SlashCommand { Command = "/asyncCommand" };
            var responder = Substitute.For<Responder<SlashCommandResponse>>();

            var sut = Configure(c => c
                .RegisterSlashCommandHandler("/command", handler)
                .RegisterAsyncSlashCommandHandler("/asyncCommand", asyncHandler));

            // Act
            HandleSlashCommands(sut, responder, new[] { otherSlashCommand, slashCommand, asyncSlashCommand });

            // Assert
            handler.DidNotReceive().Handle(otherSlashCommand);
            handler.Received().Handle(slashCommand);
            handler.DidNotReceive().Handle(asyncSlashCommand);

            asyncHandler.DidNotReceive().Handle(otherSlashCommand, responder);
            asyncHandler.DidNotReceive().Handle(slashCommand, responder);
            asyncHandler.Received().Handle(asyncSlashCommand, responder);
        }

        [Test]
        public void RegisterWorkflowStepEditHandler()
        {
            // Arrange
            var keyedHandler = Substitute.For<IWorkflowStepEditHandler>();
            var genericHandler = Substitute.For<IWorkflowStepEditHandler>();
            var keyedAsyncHandler = Substitute.For<IAsyncWorkflowStepEditHandler>();
            var genericAsyncHandler = Substitute.For<IAsyncWorkflowStepEditHandler>();
            var otherShortcut = new WorkflowStepEdit { CallbackId = "other" };
            var shortcut = new WorkflowStepEdit { CallbackId = "key" };
            var responder = Substitute.For<Responder>();

            var sut = Configure(c => c
                .RegisterWorkflowStepEditHandler("key", keyedHandler)
                .RegisterWorkflowStepEditHandler(genericHandler)
                .RegisterAsyncWorkflowStepEditHandler("key", keyedAsyncHandler)
                .RegisterAsyncWorkflowStepEditHandler(genericAsyncHandler));

            // Act
            HandleWorkflowStepEdits(sut, responder, new[] { otherShortcut, shortcut });

            // Assert
            keyedHandler.DidNotReceive().Handle(otherShortcut);
            keyedHandler.Received().Handle(shortcut);

            genericHandler.Received().Handle(otherShortcut);
            genericHandler.Received().Handle(shortcut);

            keyedAsyncHandler.DidNotReceive().Handle(otherShortcut, responder);
            keyedAsyncHandler.Received().Handle(shortcut, responder);

            genericAsyncHandler.Received().Handle(otherShortcut, responder);
            genericAsyncHandler.Received().Handle(shortcut, responder);
        }

        [Test]
        public void RegisterInteractiveMessageHandler()
        {
            // Arrange
            var handler = Substitute.For<IInteractiveMessageHandler>();
            var otherInteractiveMessage = new InteractiveMessage { Actions = { new Interaction.Button { Name = "other" } } };
            var interactiveMessage = new InteractiveMessage { Actions = { new Interaction.Button { Name = "action" } } };

            var sut = Configure(c => c.RegisterInteractiveMessageHandler("action", handler));

            // Act
            HandleLegacyInteractiveMessages(sut, new[] { otherInteractiveMessage, interactiveMessage });

            // Assert
            handler.DidNotReceive().Handle(otherInteractiveMessage);
            handler.Received().Handle(interactiveMessage);
        }

        [Test]
        public void RegisterOptionProvider()
        {
            // Arrange
            var handler = Substitute.For<IOptionProvider>();
            var otherOptionsRequest = new OptionsRequest { Name = "other" };
            var optionsRequest = new OptionsRequest { Name = "action" };

            var sut = Configure(c => c.RegisterOptionProvider("action", handler));

            // Act
            HandleLegacyOptionsRequest(sut, new[] { otherOptionsRequest, optionsRequest });

            // Assert
            handler.DidNotReceive().GetOptions(otherOptionsRequest);
            handler.Received().GetOptions(optionsRequest);
        }

        [Test]
        public void RegisterDialogSubmissionHandler_HandleSubmissions()
        {
            // Arrange
            var handler = Substitute.For<IDialogSubmissionHandler>();
            var otherDialogSubmission = new DialogSubmission { CallbackId = "other" };
            var dialogSubmission = new DialogSubmission { CallbackId = "key" };

            var sut = Configure(c => c.RegisterDialogSubmissionHandler("key", handler));

            // Act
            HandleLegacyDialogSubmissions(sut, new[] { otherDialogSubmission, dialogSubmission });

            // Assert
            handler.DidNotReceive().Handle(otherDialogSubmission);
            handler.Received().Handle(dialogSubmission);
        }

        [Test]
        public void RegisterDialogSubmissionHandler_HandleCancellations()
        {
            // Arrange
            var handler = Substitute.For<IDialogSubmissionHandler>();
            var otherDialogCancel = new DialogCancellation { CallbackId = "other" };
            var dialogCancel = new DialogCancellation { CallbackId = "key" };

            var sut = Configure(c => c.RegisterDialogSubmissionHandler("key", handler));

            // Act
            HandleLegacyDialogCancellations(sut, new[] { otherDialogCancel, dialogCancel });

            // Assert
            handler.DidNotReceive().HandleCancel(otherDialogCancel);
            handler.Received().HandleCancel(dialogCancel);
        }

        private void UseService<TService>(TService service, Action<TConfig, Func<ISlackServiceProvider, TService>> registerFactory, Func<ISlackServiceProvider, TService> getService) where TService : class
        {
            var serviceFactory = Substitute.For<Func<ISlackServiceProvider, TService>>();
            serviceFactory(Arg.Any<ISlackServiceProvider>()).Returns(service);

            var sut = Configure(c => registerFactory(c, serviceFactory));

            ShouldBeSingleInstance(sut, getService, service);

            serviceFactory.Received(1)(sut); // Service should only be created once
        }

        private static void ShouldBeSingleInstance<TService>(ISlackServiceProvider sut, Func<ISlackServiceProvider, TService> getService, TService service) where TService : class
        {
            getService(sut).ShouldBe(service);
            getService(sut).ShouldBe(service, "Should be same instance");

            DuringRequest(sut, _ => getService(sut).ShouldBe(service, "Should be same instance during request"));
        }

        protected void HandleEvents(ISlackServiceProvider services, EventCallback[] eventCallbacks) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateEventHandler(ctx), (h, e) => h.Handle(e), eventCallbacks);

        protected void HandleBlockActions(ISlackServiceProvider services, Responder responder, BlockActionRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateBlockActionHandler(ctx), (h, r) => h.Handle(r, responder), requests);

        protected void HandleBlockOptionRequests(ISlackServiceProvider services, BlockOptionsRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateBlockOptionProvider(ctx), (h, r) => h.GetOptions(r), requests);

        protected void HandleMessageShortcuts(ISlackServiceProvider services, Responder responder, MessageShortcut[] shortcuts) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateMessageShortcutHandler(ctx), (h, s) => h.Handle(s, responder), shortcuts);

        protected void HandleGlobalShortcuts(ISlackServiceProvider services, Responder responder, GlobalShortcut[] shortcuts) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx), (h, s) => h.Handle(s, responder), shortcuts);

        protected void HandleViewSubmissions(ISlackServiceProvider services, Responder<ViewSubmissionResponse> responder, ViewSubmission[] submissions) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateViewSubmissionHandler(ctx), (h, s) => h.Handle(s, responder), submissions);

        protected void HandleViewCloses(ISlackServiceProvider services, Responder responder, ViewClosed[] closes) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateViewSubmissionHandler(ctx), (h, c) => h.HandleClose(c, responder), closes);

        protected void HandleSlashCommands(ISlackServiceProvider services, Responder<SlashCommandResponse> responder, SlashCommand[] commands) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateSlashCommandHandler(ctx), (h, c) => h.Handle(c, responder), commands);

        protected void HandleWorkflowStepEdits(ISlackServiceProvider services, Responder responder, WorkflowStepEdit[] edits) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateWorkflowStepEditHandler(ctx), (h, e) => h.Handle(e, responder), edits);

        protected void HandleLegacyInteractiveMessages(ISlackServiceProvider services, InteractiveMessage[] messages) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx), (h, m) => h.Handle(m), messages);

        protected void HandleLegacyOptionsRequest(ISlackServiceProvider services, OptionsRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyOptionProvider(ctx), (h, r) => h.GetOptions(r), requests);

        protected void HandleLegacyDialogSubmissions(ISlackServiceProvider services, DialogSubmission[] submissions) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx), (h, s) => h.Handle(s), submissions);

        protected void HandleLegacyDialogCancellations(ISlackServiceProvider services, DialogCancellation[] cancellations) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx), (h, c) => h.HandleCancel(c), cancellations);

        private void HandleInRequest<THandler, TInput>(ISlackServiceProvider services, Func<ISlackHandlerFactory, SlackRequestContext, THandler> createHandler, Action<THandler, TInput> handle, TInput[] inputs)
        {
            DuringRequest(services, ctx =>
                {
                    var handler = createHandler(services.GetHandlerFactory(), ctx);
                    foreach (var input in inputs)
                        handle(handler, input);
                });
        }

        private void ReplaceRequestHandling<THandler>(
            Action<TConfig, Func<SlackRequestContext, THandler>> replaceHandling,
            Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
            where THandler : class
        {
            var replacementHandler1 = Substitute.For<THandler>();
            var replacementHandler2 = Substitute.For<THandler>();
            var handlerFactory = Substitute.For<Func<SlackRequestContext, THandler>>();
            handlerFactory(Arg.Any<SlackRequestContext>())
                .Returns(replacementHandler1, replacementHandler2);

            var sut = Configure(c => replaceHandling(c, handlerFactory));

            DuringRequest(sut, ctx => getHandler(sut.GetHandlerFactory(), ctx).ShouldBe(replacementHandler1));
            DuringRequest(sut, ctx => getHandler(sut.GetHandlerFactory(), ctx).ShouldBe(replacementHandler2, "Should be different instance for different request"));
        }

        protected static void DuringRequest(ISlackServiceProvider services, Action<SlackRequestContext> duringRequest)
        {
            var requestContext = services.GetRequestContextFactory().CreateRequestContext();
            var requestScope = requestContext.BeginRequest(services.GetRequestListener());

            try
            {
                duringRequest(requestContext);
            }
            finally
            {
                requestScope.DisposeAsync().AsTask().ShouldComplete();
            }
        }

        protected virtual ISlackServiceProvider DefaultServiceFactory() => Configure(_ => { });

        protected abstract ISlackServiceProvider Configure(Action<TConfig> configure);
    }
}
