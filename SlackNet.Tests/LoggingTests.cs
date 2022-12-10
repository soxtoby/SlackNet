#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Handlers;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Tests;

public class LoggingTests
{
    private TestLogger _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new TestLogger();
    }

    [Test]
    public void Data_LogsDataCategoryEvent()
    {
        var value = 1;

        _sut.Data("test {Value}", value);

        _sut.Event.Category.ShouldBe(LogCategory.Data);
        _sut.Event.MessageTemplate.ShouldBe("test {Value}");
        _sut.Event.Properties.ShouldOnlyContain(new[] { new KeyValuePair<string, object?>("Value", value) });
    }

    [Test]
    public void Internal_NoException_LogsInternalCategoryEvent()
    {
        var value = 1;

        _sut.Internal("test {Value}", value);

        _sut.Event.Category.ShouldBe(LogCategory.Internal);
        _sut.Event.MessageTemplate.ShouldBe("test {Value}");
        _sut.Event.Properties.ShouldOnlyContain(new[] { new KeyValuePair<string, object?>("Value", value) });
        _sut.Event.Exception.ShouldBeNull();
    }

    [Test]
    public void Internal_WithException_LogsInternalCategoryEventWithException()
    {
        var value = 1;
        var expectedException = new Exception("expected");

        _sut.Internal(expectedException, "test {Value}", value);

        _sut.Event.Category.ShouldBe(LogCategory.Internal);
        _sut.Event.MessageTemplate.ShouldBe("test {Value}");
        _sut.Event.Properties.ShouldOnlyContain(new[] { new KeyValuePair<string, object?>("Value", value) });
        _sut.Event.Exception.ShouldBe(expectedException);
    }

    [Test]
    public void Request_LogsRequestCategoryEvent()
    {
        var value = 1;
        _sut.Request("test {Value}", value);

        _sut.Event.Category.ShouldBe(LogCategory.Request);
        _sut.Event.MessageTemplate.ShouldBe("test {Value}");
        _sut.Event.Properties.ShouldOnlyContain(new[] { new KeyValuePair<string, object?>("Value", value) });
    }

    [Test]
    public void Error_LogsErrorCategoryEventWithException()
    {
        var value = 1;
        var expectedException = new Exception("expected");

        _sut.Error(expectedException, "test {Value}", value);

        _sut.Event.Category.ShouldBe(LogCategory.Error);
        _sut.Event.MessageTemplate.ShouldBe("test {Value}");
        _sut.Event.Properties.ShouldOnlyContain(new[] { new KeyValuePair<string, object?>("Value", value) });
        _sut.Event.Exception.ShouldBe(expectedException);
    }

    [Test]
    public void SlackRequestContextAvailable_RequestIdPropertyIncluded()
    {
        try
        {
            SlackRequestContext.Current = new SlackRequestContext { [nameof(SlackRequestContext.RequestId)] = "expected value" };

            _sut.Data("test");

            _sut.Event.Properties[nameof(SlackRequestContext.RequestId)].ShouldBe("expected value");
        }
        finally
        {
            SlackRequestContext.Current = null;
        }
    }

    [Test]
    public void WithContext_AddsExtraProperties()
    {
        var value = 1;
        var extraValue = 2;

        _sut.WithContext("ExtraValue", extraValue)
            .Data("test {Value}", value);

        _sut.Event.Properties.ShouldOnlyContain(new[]
            {
                new KeyValuePair<string, object?>("Value", value),
                new KeyValuePair<string, object?>("ExtraValue", extraValue)
            });
    }

    [Test]
    public void WithContext_OverridesPreviousContext()
    {
        var oldValue = 1;
        var newValue = 2;

        _sut.WithContext("Value", oldValue)
            .WithContext("Value", newValue)
            .Data("test");

        _sut.Event.Properties["Value"].ShouldBe(newValue);
    }

    [Test]
    public void ForSource_AddTypeNameProperty()
    {
        var value = 1;

        _sut.ForSource<LoggingTests>()
            .Data("test {Value}", value);

        _sut.Event.Properties.ShouldOnlyContain(new[]
            {
                new KeyValuePair<string, object?>("Value", value),
                new KeyValuePair<string, object?>("Source", typeof(LoggingTests).FullName) // Should be a string, so source can be used for filtering
            });
    }

    [Test]
    public void MessagePropertyNames_ReturnsPropertyNamesInMessage()
    {
        _sut.WithContext("Extra", 1)
            .Data("Foo: {Foo}, Bar: {Bar}", 2, 3);

        _sut.Event.MessagePropertyNames().ShouldMatch(new[] { "Foo", "Bar" });
    }

    [Test]
    public void MessagePropertyValues_ReturnsMessagePropertyValues()
    {
        _sut.WithContext("Extra", 1)
            .Data("Foo: {Foo}, Bar: {Bar}", 2, 3);

        _sut.Event.MessagePropertyValues().ShouldMatch(new[] { 2, 3 });
    }

    [Test]
    public void ExtraPropertyNames_ReturnsOrderedExtraPropertyNames()
    {
        _sut.WithContext("Baz", 1)
            .WithContext("Foo", 2)
            .WithContext("Bar", 3)
            .Data("test {Value}", 4);

        _sut.Event.ExtraPropertyNames().ShouldMatch(new[] { "Bar", "Baz", "Foo" });
    }

    [Test]
    public void FullMessageTemplate_NoException_IncludesLinesForExtraProperties()
    {
        _sut.WithContext("Extra1", 1)
            .WithContext("Extra2", 2)
            .Data("Foo: {Foo}, Bar: {Bar}", 3, 4);

        _sut.Event.FullMessageTemplate().ShouldBe("Foo: {Foo}, Bar: {Bar}"
            + Environment.NewLine + "  Extra1: {Extra1}"
            + Environment.NewLine + "  Extra2: {Extra2}");
    }

    [Test]
    public void FullMessageTemplate_WithException_IncludesLinesForExtraPropertiesAndException()
    {
        _sut.WithContext("Extra1", 1)
            .WithContext("Extra2", 2)
            .Error(new Exception(), "Foo: {Foo}, Bar: {Bar}", 3, 4);

        _sut.Event.FullMessageTemplate().ShouldBe("Foo: {Foo}, Bar: {Bar}"
            + Environment.NewLine + "  Extra1: {Extra1}"
            + Environment.NewLine + "  Extra2: {Extra2}"
            + Environment.NewLine + "  Exception: {Exception}");
    }

    [Test]
    public void IndexedMessageTemplate_ReplacesNamedPlaceholdersWithIndexes()
    {
        _sut.Data("Foo: {Foo}, Bar: {Bar}", 1, 2);

        _sut.Event.IndexedMessageTemplate().ShouldBe("Foo: {0}, Bar: {1}");
    }

    [Test]
    public void IndexedFullMessageTemplate_ReplacesNamedPlaceholdersWithIndexes()
    {
        _sut.WithContext("Extra1", 1)
            .WithContext("Extra2", 2)
            .Error(new Exception(), "Foo: {Foo}, Bar: {Bar}", 3, 4);

        _sut.Event.IndexedFullMessageTemplate().ShouldBe("Foo: {0}, Bar: {1}"
            + Environment.NewLine + "  Extra1: {2}"
            + Environment.NewLine + "  Extra2: {3}"
            + Environment.NewLine + "  Exception: {4}");
    }

    [Test]
    public void FullMessagePropertyValues_NoException_IncludesValuesInOrderOfProperties()
    {
        _sut.WithContext("Baz", "baz")
            .WithContext("Bar", "bar")
            .Data("Foo: {Foo}", "foo");

        _sut.Event.FullMessagePropertyValues().ShouldMatch(new[] { "foo", "bar", "baz" });
    }

    [Test]
    public void FullMessagePropertyValues_WithException_IncludesValuesInOrderOfProperties()
    {
        var exception = new Exception("expected");

        _sut.WithContext("Baz", "baz")
            .WithContext("Bar", "bar")
            .Error(exception, "Foo: {Foo}", "foo");

        _sut.Event.FullMessagePropertyValues().ShouldMatch(new object?[] { "foo", "bar", "baz", exception });
    }

    [Test]
    public void FullMessage_FillsInValues()
    {
        var exception = new Exception("expected");

        _sut.WithContext("Extra", "extra")
            .Error(exception, "Number: {Number}, String: {String}, Type: {Type}, Enumerable: {Enumerable}",
                1, "two", typeof(LoggingTests), new[] { "a", "b", "c" });

        _sut.Event.FullMessage().ShouldBe($"Number: 1, String: two, Type: {typeof(LoggingTests).FullName}, Enumerable: a, b, c"
            + Environment.NewLine + "  Extra: extra"
            + Environment.NewLine + $"  Exception: {exception}");
    }

    [Test]
    public void RequestHandler_LogsInnerHandlers()
    {
        var eventHandler = new CompositeEventHandler(new[]
            {
                new TestEventHandler(),
                new TestMessageEventHandler().ToEventHandler()
            });

        var blockActionHandler = new CompositeBlockActionHandler(new[]
            {
                new TestAsyncBlockActionHandler(),
                new TestBlockActionHandler().ToBlockActionHandler(),
                new TestAsyncButtonActionHandler().ToBlockActionHandler(),
                new TestButtonActionHandler().ToBlockActionHandler(),
                new TestSpecificButtonActionHandler().ToBlockActionHandler("matching")
            });

        var interactiveMessageHandler = new SwitchingInteractiveMessageHandler(new TestHandlerIndex<IInteractiveMessageHandler>
            {
                ["matching"] = new TestInteractiveMessageHandler()
            });

        var dialogSubmissionHandler = new SwitchingDialogSubmissionHandler(new TestHandlerIndex<IDialogSubmissionHandler>
            {
                ["matching"] = new TestDialogSubmissionHandler()
            });

        var messageShortcutHandler = new CompositeMessageShortcutHandler(new[]
            {
                new TestAsyncMessageShortcutHandler(),
                new TestMessageShortcutHandler().ToMessageShortcutHandler(),
                new TestSpecificMessageShortcutHandler().ToMessageShortcutHandler("matching")
            });

        var globalShortcutHandler = new CompositeGlobalShortcutHandler(new[]
            {
                new TestAsyncGlobalShortcutHandler(),
                new TestGlobalShortcutHandler().ToGlobalShortcutHandler(),
                new TestSpecificGlobalShortcutHandler().ToGlobalShortcutHandler("matching")
            });

        var viewSubmissionHandler = new SwitchingViewSubmissionHandler(new TestHandlerIndex<IAsyncViewSubmissionHandler>
            {
                ["async"] = new TestAsyncViewSubmissionHandler(),
                ["sync"] = new TestViewSubmissionHandler().ToViewSubmissionHandler()
            });

        var workflowStepEditHandler = new CompositeWorkflowStepEditHandler(new[]
            {
                new TestAsyncWorkflowStepEditHandler(),
                new TestWorkflowStepEditHandler().ToWorkflowStepEditHandler(),
                new TestSpecificWorkflowStepEditHandler().ToWorkflowStepEditHandler("matching")
            });

        var blockOptionProvider = new SwitchingBlockOptionProvider(new TestHandlerIndex<IBlockOptionProvider>
            {
                ["matching"] = new TestBlockOptionProvider()
            });

        var legacyOptionProvider = new SwitchingOptionProvider(new TestHandlerIndex<IOptionProvider>
            {
                ["matching"] = new TestLegacyOptionProvider()
            });

        var slashCommandHandler = new SwitchingSlashCommandHandler(new TestHandlerIndex<IAsyncSlashCommandHandler>
            {
                ["/async"] = new TestAsyncSlashCommandHandler(),
                ["/sync"] = new TestSlashCommandHandler().ToSlashCommandHandler()
            });

        _sut.RequestHandler(eventHandler, new EventCallback { Event = new Hello() }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestEventHandler).FullName}");

        _sut.RequestHandler(eventHandler, new EventCallback { Event = new MessageEvent() }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestEventHandler).FullName}, {typeof(TestMessageEventHandler).FullName}");

        _sut.RequestHandler(blockActionHandler, new BlockActionRequest { Actions = { new StaticSelectAction() } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncBlockActionHandler).FullName}, {typeof(TestBlockActionHandler).FullName}");

        _sut.RequestHandler(blockActionHandler, new BlockActionRequest { Actions = { new ButtonAction() } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncBlockActionHandler).FullName}, {typeof(TestBlockActionHandler).FullName}, {typeof(TestAsyncButtonActionHandler).FullName}, {typeof(TestButtonActionHandler).FullName}");

        _sut.RequestHandler(blockActionHandler, new BlockActionRequest { Actions = { new ButtonAction { ActionId = "matching" } } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncBlockActionHandler).FullName}, {typeof(TestBlockActionHandler).FullName}, {typeof(TestAsyncButtonActionHandler).FullName}, {typeof(TestButtonActionHandler).FullName}, {typeof(TestSpecificButtonActionHandler).FullName}");

        _sut.RequestHandler(interactiveMessageHandler, new InteractiveMessage { Action = new Interaction.Button { Name = "other" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe("Handled with <none>");

        _sut.RequestHandler(interactiveMessageHandler, new InteractiveMessage { Action = new Interaction.Button { Name = "matching" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestInteractiveMessageHandler).FullName}");

        _sut.RequestHandler(dialogSubmissionHandler, new DialogSubmission { CallbackId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe("Handled with <none>");

        _sut.RequestHandler(dialogSubmissionHandler, new DialogSubmission { CallbackId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestDialogSubmissionHandler).FullName}");

        _sut.RequestHandler(dialogSubmissionHandler, new DialogCancellation { CallbackId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe("Handled with <none>");

        _sut.RequestHandler(dialogSubmissionHandler, new DialogCancellation { CallbackId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestDialogSubmissionHandler).FullName}");

        _sut.RequestHandler(messageShortcutHandler, new MessageShortcut { CallbackId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncMessageShortcutHandler).FullName}, {typeof(TestMessageShortcutHandler).FullName}");

        _sut.RequestHandler(messageShortcutHandler, new MessageShortcut { CallbackId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncMessageShortcutHandler).FullName}, {typeof(TestMessageShortcutHandler).FullName}, {typeof(TestSpecificMessageShortcutHandler).FullName}");

        _sut.RequestHandler(globalShortcutHandler, new GlobalShortcut { CallbackId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncGlobalShortcutHandler).FullName}, {typeof(TestGlobalShortcutHandler).FullName}");

        _sut.RequestHandler(globalShortcutHandler, new GlobalShortcut { CallbackId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncGlobalShortcutHandler).FullName}, {typeof(TestGlobalShortcutHandler).FullName}, {typeof(TestSpecificGlobalShortcutHandler).FullName}");

        _sut.RequestHandler(viewSubmissionHandler, new ViewSubmission { View = new HomeViewInfo { CallbackId = "async" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncViewSubmissionHandler).FullName}");

        _sut.RequestHandler(viewSubmissionHandler, new ViewSubmission { View = new HomeViewInfo { CallbackId = "sync" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestViewSubmissionHandler).FullName}");

        _sut.RequestHandler(viewSubmissionHandler, new ViewClosed { View = new HomeViewInfo { CallbackId = "async" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncViewSubmissionHandler).FullName}");

        _sut.RequestHandler(viewSubmissionHandler, new ViewClosed { View = new HomeViewInfo { CallbackId = "sync" } }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestViewSubmissionHandler).FullName}");

        _sut.RequestHandler(workflowStepEditHandler, new WorkflowStepEdit { CallbackId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncWorkflowStepEditHandler).FullName}, {typeof(TestWorkflowStepEditHandler).FullName}");

        _sut.RequestHandler(workflowStepEditHandler, new WorkflowStepEdit { CallbackId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncWorkflowStepEditHandler).FullName}, {typeof(TestWorkflowStepEditHandler).FullName}, {typeof(TestSpecificWorkflowStepEditHandler).FullName}");

        _sut.RequestHandler(blockOptionProvider, new BlockOptionsRequest { ActionId = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe("Handled with <none>");

        _sut.RequestHandler(blockOptionProvider, new BlockOptionsRequest { ActionId = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestBlockOptionProvider).FullName}");

        _sut.RequestHandler(legacyOptionProvider, new OptionsRequest { Name = "other" }, "Handled");
        _sut.Event.FullMessage().ShouldBe("Handled with <none>");

        _sut.RequestHandler(legacyOptionProvider, new OptionsRequest { Name = "matching" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestLegacyOptionProvider).FullName}");

        _sut.RequestHandler(slashCommandHandler, new SlashCommand { Command = "/async" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestAsyncSlashCommandHandler).FullName}");

        _sut.RequestHandler(slashCommandHandler, new SlashCommand { Command = "/sync" }, "Handled");
        _sut.Event.FullMessage().ShouldBe($"Handled with {typeof(TestSlashCommandHandler).FullName}");
    }

    class TestLogger : ILogger
    {
        public ILogEvent Event { get; private set; } = null!;
        public void Log(ILogEvent logEvent) => Event = logEvent;
    }

    class TestEventHandler : IEventHandler
    {
        public async Task Handle(EventCallback eventCallback) { }
    }

    class TestMessageEventHandler : IEventHandler<MessageEvent>
    {
        public async Task Handle(MessageEvent slackEvent) { }
    }

    class TestAsyncBlockActionHandler : IAsyncBlockActionHandler
    {
        public async Task Handle(BlockActionRequest request, Responder respond) { }
    }

    class TestBlockActionHandler : IBlockActionHandler
    {
        public async Task Handle(BlockActionRequest request) { }
    }

    class TestAsyncButtonActionHandler : IAsyncBlockActionHandler<ButtonAction>
    {
        public async Task Handle(ButtonAction action, BlockActionRequest request, Responder respond) { }
    }

    class TestButtonActionHandler : IBlockActionHandler<ButtonAction>
    {
        public async Task Handle(ButtonAction action, BlockActionRequest request) { }
    }

    class TestSpecificButtonActionHandler : IAsyncBlockActionHandler<ButtonAction>
    {
        public async Task Handle(ButtonAction action, BlockActionRequest request, Responder respond) { }
    }

    class TestHandlerIndex<THandler> : Dictionary<string, THandler>, IHandlerIndex<THandler>
    {
        public bool HasHandler(string key) => ContainsKey(key);
        public bool TryGetHandler(string key, out THandler handler) => TryGetValue(key, out handler);
    }

    class TestInteractiveMessageHandler : IInteractiveMessageHandler
    {
        public async Task<MessageResponse> Handle(InteractiveMessage message) => new();
    }

    class TestDialogSubmissionHandler : IDialogSubmissionHandler
    {
        public async Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog) => Enumerable.Empty<DialogError>();
        public async Task HandleCancel(DialogCancellation cancellation) { }
    }

    class TestAsyncMessageShortcutHandler : IAsyncMessageShortcutHandler
    {
        public async Task Handle(MessageShortcut request, Responder respond) { }
    }

    class TestMessageShortcutHandler : IMessageShortcutHandler
    {
        public async Task Handle(MessageShortcut request) { }
    }

    class TestSpecificMessageShortcutHandler : IAsyncMessageShortcutHandler
    {
        public async Task Handle(MessageShortcut request, Responder respond) { }
    }

    class TestAsyncGlobalShortcutHandler : IAsyncGlobalShortcutHandler
    {
        public async Task Handle(GlobalShortcut request, Responder respond) { }
    }

    class TestGlobalShortcutHandler : IGlobalShortcutHandler
    {
        public async Task Handle(GlobalShortcut request) { }
    }

    class TestSpecificGlobalShortcutHandler : IAsyncGlobalShortcutHandler
    {
        public async Task Handle(GlobalShortcut request, Responder respond) { }
    }

    class TestAsyncViewSubmissionHandler : IAsyncViewSubmissionHandler
    {
        public async Task Handle(ViewSubmission viewSubmission, Responder<ViewSubmissionResponse> respond) { }
        public async Task HandleClose(ViewClosed viewClosed, Responder respond) { }
    }

    class TestViewSubmissionHandler : IViewSubmissionHandler
    {
        public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission) => new ClearViewsResponse();
        public async Task HandleClose(ViewClosed viewClosed) { }
    }

    class TestAsyncWorkflowStepEditHandler : IAsyncWorkflowStepEditHandler
    {
        public async Task Handle(WorkflowStepEdit workflowStepEdit, Responder respond) { }
    }

    class TestWorkflowStepEditHandler : IWorkflowStepEditHandler
    {
        public async Task Handle(WorkflowStepEdit workflowStepEdit) { }
    }

    class TestSpecificWorkflowStepEditHandler : IAsyncWorkflowStepEditHandler
    {
        public async Task Handle(WorkflowStepEdit workflowStepEdit, Responder respond) { }
    }

    class TestBlockOptionProvider : IBlockOptionProvider
    {
        public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) => new BlockOptionsResponse();
    }

    class TestLegacyOptionProvider :IOptionProvider
    {
        public async Task<OptionsResponse> GetOptions(OptionsRequest request) => new OptionsResponse();
    }

    class TestAsyncSlashCommandHandler : IAsyncSlashCommandHandler
    {
        public async Task Handle(SlashCommand command, Responder<SlashCommandResponse> respond) { }
    }

    class TestSlashCommandHandler : ISlashCommandHandler
    {
        public async Task<SlashCommandResponse> Handle(SlashCommand command) => new();
    }
}