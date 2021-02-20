using System;
using System.Collections.Generic;
using System.Linq;
using EasyAssertions;
using NSubstitute;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Tests.Configuration
{
    public abstract class SlackHandlerConfigurationBaseTests<TConfig> : SlackServiceConfigurationBaseTests<TConfig> where TConfig : SlackHandlerConfigurationBase<TConfig>
    {
        [Test]
        public void ReplaceEventHandling()
        {
            ReplaceCollectionHandling(
                (c, f) => c.ReplaceEventHandling(f),
                (c, h) => c.RegisterEventHandler(h),
                (hf, ctx) => hf.CreateEventHandler(ctx));
        }

        [Test]
        public void ReplaceBlockActionHandling()
        {
            ReplaceCollectionHandling(
                (c, f) => c.ReplaceBlockActionHandling(f),
                (c, h) => c.RegisterAsyncBlockActionHandler(h),
                (hf, ctx) => hf.CreateBlockActionHandler(ctx));
        }

        [Test]
        public void ReplaceBlockOptionProvider()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceBlockOptionProviding(f),
                (c, k, h) => c.RegisterBlockOptionProvider(k, h),
                (hf, ctx) => hf.CreateBlockOptionProvider(ctx));
        }

        [Test]
        public void ReplaceMessageShortcutHandling()
        {
            ReplaceCollectionHandling(
                (c, f) => c.ReplaceMessageShortcutHandling(f),
                (c, h) => c.RegisterAsyncMessageShortcutHandler(h),
                (hf, ctx) => hf.CreateMessageShortcutHandler(ctx));
        }

        [Test]
        public void ReplaceGlobalShortcutHandling()
        {
            ReplaceCollectionHandling(
                (c, f) => c.ReplaceGlobalShortcutHandling(f),
                (c, h) => c.RegisterAsyncGlobalShortcutHandler(h),
                (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx));
        }

        [Test]
        public void ReplaceViewSubmissionHandling()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceViewSubmissionHandling(f),
                (c, k, h) => c.RegisterAsyncViewSubmissionHandler(k, h),
                (hf, ctx) => hf.CreateViewSubmissionHandler(ctx));
        }

        [Test]
        public void ReplaceSlashCommandHandling()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceSlashCommandHandling(f),
                (c, k, h) => c.RegisterAsyncSlashCommandHandler(k, h),
                (hf, ctx) => hf.CreateSlashCommandHandler(ctx));
        }

        [Test]
        public void ReplaceWorkflowStepEditHandling()
        {
            ReplaceCollectionHandling(
                (c, f) => c.ReplaceWorkflowStepEditHandling(f),
                (c, h) => c.RegisterAsyncWorkflowStepEditHandler(h),
                (hf, ctx) => hf.CreateWorkflowStepEditHandler(ctx));
        }

        [Test]
        public void ReplaceLegacyInteractiveMessageHandling()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceLegacyInteractiveMessageHandling(f),
                (c, k, h) => c.RegisterInteractiveMessageHandler(k, h),
                (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx));
        }

        [Test]
        public void ReplaceLegacyOptionProviding()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceLegacyOptionProviding(f),
                (c, k, h) => c.RegisterOptionProvider(k, h),
                (hf, ctx) => hf.CreateLegacyOptionProvider(ctx));
        }

        [Test]
        public void ReplaceLegacyDialogSubmissionHandling()
        {
            ReplaceKeyedHandling(
                (c, f) => c.ReplaceLegacyDialogSubmissionHandling(f),
                (c, k, h) => c.RegisterDialogSubmissionHandler(k, h),
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

        protected void HandleEvents(ISlackServiceFactory services, EventCallback[] eventCallbacks) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateEventHandler(ctx), (h, e) => h.Handle(e), eventCallbacks);

        protected void HandleBlockActions(ISlackServiceFactory services, Responder responder, BlockActionRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateBlockActionHandler(ctx), (h, r) => h.Handle(r, responder), requests);

        protected void HandleBlockOptionRequests(ISlackServiceFactory services, BlockOptionsRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateBlockOptionProvider(ctx), (h, r) => h.GetOptions(r), requests);

        protected void HandleMessageShortcuts(ISlackServiceFactory services, Responder responder, MessageShortcut[] shortcuts) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateMessageShortcutHandler(ctx), (h, s) => h.Handle(s, responder), shortcuts);

        protected void HandleGlobalShortcuts(ISlackServiceFactory services, Responder responder, GlobalShortcut[] shortcuts) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateGlobalShortcutHandler(ctx), (h, s) => h.Handle(s, responder), shortcuts);

        protected void HandleViewSubmissions(ISlackServiceFactory services, Responder<ViewSubmissionResponse> responder, ViewSubmission[] submissions) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateViewSubmissionHandler(ctx), (h, s) => h.Handle(s, responder), submissions);

        protected void HandleViewCloses(ISlackServiceFactory services, Responder responder, ViewClosed[] closes) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateViewSubmissionHandler(ctx), (h, c) => h.HandleClose(c, responder), closes);

        protected void HandleSlashCommands(ISlackServiceFactory services, Responder<SlashCommandResponse> responder, SlashCommand[] commands) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateSlashCommandHandler(ctx), (h, c) => h.Handle(c, responder), commands);

        protected void HandleWorkflowStepEdits(ISlackServiceFactory services, Responder responder, WorkflowStepEdit[] edits) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateWorkflowStepEditHandler(ctx), (h, e) => h.Handle(e, responder), edits);

        protected void HandleLegacyInteractiveMessages(ISlackServiceFactory services, InteractiveMessage[] messages) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyInteractiveMessageHandler(ctx), (h, m) => h.Handle(m), messages);

        protected void HandleLegacyOptionsRequest(ISlackServiceFactory services, OptionsRequest[] requests) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyOptionProvider(ctx), (h, r) => h.GetOptions(r), requests);

        protected void HandleLegacyDialogSubmissions(ISlackServiceFactory services, DialogSubmission[] submissions) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx), (h, s) => h.Handle(s), submissions);

        protected void HandleLegacyDialogCancellations(ISlackServiceFactory services, DialogCancellation[] cancellations) =>
            HandleInRequest(services, (hf, ctx) => hf.CreateLegacyDialogSubmissionHandler(ctx), (h, c) => h.HandleCancel(c), cancellations);

        protected void HandleInRequest<THandler, TInput>(ISlackServiceFactory services, Func<ISlackHandlerFactory, SlackRequestContext, THandler> createHandler, Action<THandler, TInput> handle, TInput[] inputs)
        {
            DuringRequest(services, ctx =>
                {
                    var handler = createHandler(services.GetHandlerFactory(), ctx);
                    foreach (var input in inputs)
                        handle(handler, input);
                });
        }

        protected virtual void ReplaceCollectionHandling<THandler>(
            Action<TConfig, CollectionHandlerFactory<THandler>> replaceHandling,
            Action<TConfig, THandler> registerHandler,
            Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
            where THandler : class
        {
            var replacementHandler1 = Substitute.For<THandler>();
            var replacementHandler2 = Substitute.For<THandler>();
            var registeredHandler1 = Substitute.For<THandler>();
            var registeredHandler2 = Substitute.For<THandler>();
            var handlerFactory = Substitute.For<CollectionHandlerFactory<THandler>>();
            handlerFactory(Arg.Is<IEnumerable<THandler>>(hs => hs.SequenceEqual(new[] { registeredHandler1, registeredHandler2 })))
                .Returns(replacementHandler1, replacementHandler2);

            var sut = Configure(c =>
                {
                    replaceHandling(c, handlerFactory);
                    registerHandler(c, registeredHandler1);
                    registerHandler(c, registeredHandler2);
                });

            DuringRequest(sut, c =>
                {
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler1);
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler1, "Should be same instance within request");
                });

            DuringRequest(sut, c =>
                {
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler2, "Should be different instance for different request");
                });
        }

        protected virtual void ReplaceKeyedHandling<THandler>(
            Action<TConfig, KeyedHandlerFactory<THandler>> replaceHandling,
            Action<TConfig, string, THandler> registerHandler,
            Func<ISlackHandlerFactory, SlackRequestContext, THandler> getHandler)
            where THandler : class
        {
            var replacementHandler1 = Substitute.For<THandler>();
            var replacementHandler2 = Substitute.For<THandler>();
            var registeredHandler1 = Substitute.For<THandler>();
            var registeredHandler2 = Substitute.For<THandler>();
            var handlerFactory = Substitute.For<KeyedHandlerFactory<THandler>>();
            handlerFactory(Arg.Do((IHandlerIndex<THandler> index) =>
                    {
                        index.TryGetValue("/key1", out var h1).ShouldBe(true);
                        h1.ShouldBe(registeredHandler1);
                        index.TryGetValue("/key2", out var h2).ShouldBe(true);
                        h2.ShouldBe(registeredHandler2);
                        index.TryGetValue("/key3", out var h3).ShouldBe(false);
                        h3.ShouldBeNull();
                    }))
                .Returns(replacementHandler1, replacementHandler2);

            var sut = Configure(c =>
                {
                    replaceHandling(c, handlerFactory);
                    registerHandler(c, "/key1", registeredHandler1);
                    registerHandler(c, "/key2", registeredHandler2);
                });

            DuringRequest(sut, c =>
                {
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler1);
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler1, "Should be same instance within request");
                });

            DuringRequest(sut, c =>
                {
                    getHandler(sut.GetHandlerFactory(), c)
                        .ShouldBe(replacementHandler2, "Should be different instance for different request");
                });
        }
    }
}