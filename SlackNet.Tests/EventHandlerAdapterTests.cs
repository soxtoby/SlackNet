using System;
using System.Threading.Tasks;
using EasyAssertions;
using NUnit.Framework;
using SlackNet.Events;
using SlackNet.Handlers;
using Experimental = SlackNet.Handlers.Experimental;

namespace SlackNet.Tests;

public class EventHandlerAdapterTests
{
    [Test]
    public async Task Acknowledge_AlreadyAcknowledged_RejectsDuplicateWithoutSendingAnotherResponse()
    {
        var responses = 0;
        var responseComplete = new TaskCompletionSource<bool>();
        var sut = new EventContext(new EventCallback(),
            () =>
                {
                    responses++;
                    return responseComplete.Task;
                });

        var result = sut.Acknowledge();

        sut.IsAcknowledged.ShouldBe(true);
        result.IsCompleted.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => sut.Acknowledge());
        responses.ShouldBe(1);

        responseComplete.SetResult(true);
        await result;
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ToEventHandler_SharedAcknowledgement_WaitsBeforeInvokingLegacyHandlers(bool contextHandlerAcknowledges)
    {
        var responseComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var responses = 0;
        var callback = new EventCallback { Event = new MessageEvent() };
        var context = new EventContext(
            callback,
            () =>
                {
                    responses++;
                    return responseComplete.Task;
                });
        var legacy = new RecordingEventHandler();
        var typedLegacy = new RecordingEventHandler<MessageEvent>();
        var contextHandler = new ContextEventHandler<Event>(c => contextHandlerAcknowledges ? c.Acknowledge() : Task.CompletedTask);
        var sut = new CompositeContextEventHandler([contextHandler, legacy.ToEventHandler(), typedLegacy.ToEventHandler()]);

        var result = sut.Handle(context);

        responses.ShouldBe(1);
        legacy.Callbacks.ShouldBeEmpty();
        typedLegacy.Events.ShouldBeEmpty();
        result.IsCompleted.ShouldBe(false);

        responseComplete.SetResult(true);
        await result.WaitAsync(TimeSpan.FromSeconds(5));

        legacy.Callbacks.ShouldMatchReferences(new[] { callback });
        typedLegacy.Events.ShouldMatchReferences(new[] { (MessageEvent)callback.Event });
        responses.ShouldBe(1);
    }

    [Test]
    public async Task ToEventHandler_NonmatchingEvent_DoesNotInvokeOrAcknowledge()
    {
        var responses = 0;
        var context = new EventContext(
            new EventCallback { Event = new MessageEvent() },
            () =>
                {
                    responses++;
                    return Task.CompletedTask;
                });
        var legacy = new RecordingEventHandler<Hello>();
        var contextHandler = new ContextEventHandler<Hello>(_ => throw new InvalidOperationException("Unexpected event"));
        var sut = new CompositeContextEventHandler([legacy.ToEventHandler(), contextHandler.ToEventHandler()]);

        await sut.Handle(context);

        context.IsAcknowledged.ShouldBe(false);
        responses.ShouldBe(0);
        legacy.Events.ShouldBeEmpty();
    }

    [Test]
    public async Task ToEventHandler_TypedHandler_PreservesCallbackAndAcknowledgement()
    {
        var callback = new EventCallback { EventId = "event", Event = new MessageEvent { Text = "message" } };
        var context = new EventContext(callback, () => Task.CompletedTask);
        Experimental.IEventContext<MessageEvent> result = null;
        var handler = new ContextEventHandler<MessageEvent>(c =>
            {
                result = c;
                return c.Acknowledge();
            });
        var sut = handler.ToEventHandler();

        await sut.Handle(context);

        result.ShouldNotBeNull().And(c =>
            {
                c.Callback.ShouldReferTo(callback);
                c.Event.ShouldReferTo((MessageEvent)callback.Event);
                c.IsAcknowledged.ShouldBe(true);
            });
        context.IsAcknowledged.ShouldBe(true);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ToEventHandler_DerivedLegacyHandler_PreservesCustomHandling(bool typed)
    {
        var handled = false;
        IEventHandler handler = typed
            ? new DerivedTypedEventHandler(() => handled = true)
            : new DerivedCompositeEventHandler(() => handled = true);
        var responseComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = new EventContext(new EventCallback { Event = new MessageEvent() }, () => responseComplete.Task);
        var sut = handler.ToEventHandler();

        var result = sut.Handle(context);

        handled.ShouldBe(false);

        responseComplete.SetResult(true);
        await result.WaitAsync(TimeSpan.FromSeconds(5));

        handled.ShouldBe(true);
    }

    [Test]
    public async Task TypedEventHandler_MixedEventTypes_HandlesMatchingEvents()
    {
        var handler = new RecordingEventHandler<MessageEvent>();
        var message = new MessageEvent();
        var sut = new TypedEventHandler<MessageEvent>(handler);

        await sut.Handle(new EventCallback { Event = new Hello() });
        await sut.Handle(new EventCallback { Event = message });

        handler.Events.ShouldMatchReferences(new[] { message });
    }

    class DerivedTypedEventHandler(Action handle) : TypedEventHandler<MessageEvent>(new RecordingEventHandler<MessageEvent>()), IEventHandler
    {
        Task IEventHandler.Handle(EventCallback callback)
        {
            handle();
            return base.Handle(callback);
        }
    }

    class DerivedCompositeEventHandler(Action handle) : CompositeEventHandler(Array.Empty<IEventHandler>()), IEventHandler
    {
        Task IEventHandler.Handle(EventCallback callback)
        {
            handle();
            return base.Handle(callback);
        }
    }
}
