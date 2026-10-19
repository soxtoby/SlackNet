using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SlackNet.Events;
using SlackNet.Handlers;
using Experimental = SlackNet.Handlers.Experimental;

namespace SlackNet.Tests;

public class EventHandlerAdapterTests
{
    [TestCase(false)]
    [TestCase(true)]
    public async Task ToFunctionHandler_NonmatchingEvent_DoesNotInvokeOrAcknowledge(bool typedInputs)
    {
        var handler = new RecordingContextEventHandler<FunctionExecuted>();
        var typedHandler = new RecordingContextEventHandler<FunctionExecuted<object>>();
        var sut = typedInputs
            ? typedHandler.ToFunctionHandler("key", Default.JsonSettings())
            : handler.ToFunctionHandler("key");
        Event[] events =
            [
                new Hello(),
                new FunctionExecuted(),
                new FunctionExecuted { Function = new Function { CallbackId = "other" } }
            ];

        foreach (var slackEvent in events)
        {
            var context = new EventContext(new EventCallback { Event = slackEvent },
                () => throw new InvalidOperationException("Unexpected acknowledgement"));

            await sut.Handle(context);

            context.IsAcknowledged.ShouldBe(false);
        }

        handler.Contexts.ShouldBeEmpty();
        typedHandler.Contexts.ShouldBeEmpty();
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ToFunctionHandler_MatchingEvent_PreservesCallbackAndAcknowledgement(bool typedInputs, bool acknowledgeOriginal)
    {
        var responseComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var responses = 0;
        var callback = new EventCallback { Event = new FunctionExecuted { Function = new Function { CallbackId = "key" } } };
        var context = new EventContext(
            callback,
            () =>
                {
                    responses++;
                    return responseComplete.Task;
                });
        var handler = new RecordingContextEventHandler<FunctionExecuted>();
        var typedHandler = new RecordingContextEventHandler<FunctionExecuted<object>>();
        var sut = typedInputs
            ? typedHandler.ToFunctionHandler("key", Default.JsonSettings())
            : handler.ToFunctionHandler("key");

        await sut.Handle(context);

        IEnumerable<Experimental.IEventContext<Event>> contexts = typedInputs ? typedHandler.Contexts : handler.Contexts;
        contexts.ShouldBeSingular().And(received =>
            {
                var result = received.Single();
                result.Callback.ShouldReferTo(callback);
                result.IsAcknowledged.ShouldBe(false);
            });
        context.IsAcknowledged.ShouldBe(false);

        var acknowledgement = acknowledgeOriginal ? context.Acknowledge() : contexts.Single().Acknowledge();

        contexts.Single().IsAcknowledged.ShouldBe(true);
        context.IsAcknowledged.ShouldBe(true);
        responses.ShouldBe(1);
        acknowledgement.IsCompleted.ShouldBe(false);
        Should.Throw<InvalidOperationException>(() => acknowledgeOriginal ? contexts.Single().Acknowledge() : context.Acknowledge());

        responseComplete.SetResult(true);
        await acknowledgement.WaitAsync(TimeSpan.FromSeconds(5));
    }

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


    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ToEventHandler_NotAcknowledged_AcknowledgesBeforeInvokingLegacyHandler(bool genericContext, bool typedHandler)
    {
        var responseComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var responses = 0;
        var callback = new EventCallback { Event = new MessageEvent() };
        Func<Task> acknowledge = () =>
            {
                responses++;
                return responseComplete.Task;
            };
        Experimental.IEventContext<Event> context = genericContext
            ? new EventContext<MessageEvent>(new EventContext(callback, acknowledge), (MessageEvent)callback.Event)
            : new EventContext(callback, acknowledge);
        var legacy = new RecordingEventHandler();
        var typedLegacy = new RecordingEventHandler<MessageEvent>();
        var sut = typedHandler ? typedLegacy.ToEventHandler() : legacy.ToEventHandler();

        var result = sut.Handle(context);

        responses.ShouldBe(1);
        context.IsAcknowledged.ShouldBe(true);
        legacy.Callbacks.ShouldBeEmpty();
        typedLegacy.Events.ShouldBeEmpty();
        result.IsCompleted.ShouldBe(false);

        responseComplete.SetResult(true);
        await result.WaitAsync(TimeSpan.FromSeconds(5));

        if (typedHandler)
            typedLegacy.Events.ShouldMatchReferences(new[] { (MessageEvent)callback.Event });
        else
            legacy.Callbacks.ShouldMatchReferences(new[] { callback });
    }

    [Test]
    public async Task ToEventHandler_AlreadyAcknowledged_InvokesLegacyHandlersWithoutAcknowledgingAgain()
    {
        var responses = 0;
        var callback = new EventCallback { Event = new MessageEvent() };
        var context = new EventContext(
            callback,
            () =>
                {
                    responses++;
                    return Task.CompletedTask;
                });
        var legacy = new RecordingEventHandler();
        var typedLegacy = new RecordingEventHandler<MessageEvent>();
        var contextHandler = new ContextEventHandler<Event>(c => c.Acknowledge());
        var sut = new CompositeContextEventHandler([contextHandler, legacy.ToEventHandler(), typedLegacy.ToEventHandler()]);

        await sut.Handle(context);

        responses.ShouldBe(1);
        legacy.Callbacks.ShouldMatchReferences(new[] { callback });
        typedLegacy.Events.ShouldMatchReferences(new[] { (MessageEvent)callback.Event });
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

    [Test]
    public async Task ToFunctionHandler_ComposedTypedHandlers_PreservesConvertedInputsAndSharesAcknowledgement()
    {
        var callback = new EventCallback
            {
                Event = new FunctionExecuted
                    {
                        Function = new Function { CallbackId = "key" },
                        Inputs = JObject.Parse("""{"user_id":"U123","count":42}""")
                    }
            };
        var responses = 0;
        var context = new EventContext(
            callback,
            () =>
                {
                    responses++;
                    return Task.CompletedTask;
                });
        var rawHandler = new RecordingContextEventHandler<FunctionExecuted>();
        var userHandler = new RecordingContextEventHandler<FunctionExecuted<UserInputs>>();
        var countHandler = new RecordingContextEventHandler<FunctionExecuted<CountInputs>>();
        Experimental.IEventHandler<FunctionExecuted<UserInputs>> composedUserHandler = userHandler.ToEventHandler();
        var sut = new CompositeContextEventHandler(
            [
                rawHandler.ToFunctionHandler("key"),
                composedUserHandler.ToFunctionHandler("key", Default.JsonSettings()),
                countHandler.ToFunctionHandler("key", Default.JsonSettings())
            ]);

        await sut.Handle(context);

        rawHandler.Contexts.ShouldBeSingular()
            .And.Single().Event.ShouldReferTo((FunctionExecuted)callback.Event);
        userHandler.Contexts.ShouldBeSingular().And(contexts =>
            {
                var result = contexts.Single();
                result.Event.Inputs.UserId.ShouldBe("U123");
                result.Callback.ShouldReferTo(callback);
            });
        countHandler.Contexts.ShouldBeSingular().And(contexts =>
            {
                var result = contexts.Single();
                result.Event.Inputs.Count.ShouldBe(42);
                result.Callback.ShouldReferTo(callback);
            });
        responses.ShouldBe(0);

        await userHandler.Contexts.Single().Acknowledge();

        context.IsAcknowledged.ShouldBe(true);
        rawHandler.Contexts.Single().IsAcknowledged.ShouldBe(true);
        countHandler.Contexts.Single().IsAcknowledged.ShouldBe(true);
        responses.ShouldBe(1);
        Should.Throw<InvalidOperationException>(() => countHandler.Contexts.Single().Acknowledge());
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ToEventHandler_CustomContext_PreservesItsEventAndAcknowledgement(bool alreadyAcknowledged)
    {
        var callback = new EventCallback { Event = new MessageEvent { Text = "original" } };
        var responses = 0;
        var context = new ExternalEventContext(
            callback,
            new MessageEvent { Text = "converted" },
            () =>
                {
                    responses++;
                    return Task.CompletedTask;
                });
        if (alreadyAcknowledged)
            await context.Acknowledge();

        var handler = new RecordingContextEventHandler<MessageEvent>();
        var sut = handler.ToEventHandler();

        await sut.Handle(context);

        handler.Contexts.ShouldBeSingular().And(
            contexts =>
            {
                var result = contexts.Single();
                result.Callback.ShouldReferTo(callback);
                result.Event.Text.ShouldBe("converted");
                result.IsAcknowledged.ShouldBe(alreadyAcknowledged);
            });

        if (!alreadyAcknowledged)
            await handler.Contexts.Single().Acknowledge();

        context.IsAcknowledged.ShouldBe(true);
        handler.Contexts.Single().IsAcknowledged.ShouldBe(true);
        responses.ShouldBe(1);
        Should.Throw<InvalidOperationException>(() => handler.Contexts.Single().Acknowledge());
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ToEventHandler_CustomContext_AcknowledgesOnceBeforeInvokingLegacyHandler(bool alreadyAcknowledged, bool typedHandler)
    {
        var responseComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var callback = new EventCallback { Event = new MessageEvent() };
        var responses = 0;
        var context = new ExternalEventContext(
            callback,
            callback.Event,
            () =>
                {
                    responses++;
                    return responseComplete.Task;
                });
        if (alreadyAcknowledged)
        {
            responseComplete.SetResult(true);
            await context.Acknowledge();
        }

        var legacy = new RecordingEventHandler();
        var typedLegacy = new RecordingEventHandler<MessageEvent>();
        var sut = typedHandler ? typedLegacy.ToEventHandler() : legacy.ToEventHandler();

        var result = sut.Handle(context);

        context.IsAcknowledged.ShouldBe(true);
        responses.ShouldBe(1);
        if (!alreadyAcknowledged)
        {
            legacy.Callbacks.ShouldBeEmpty();
            typedLegacy.Events.ShouldBeEmpty();
            result.IsCompleted.ShouldBe(false);

            responseComplete.SetResult(true);
        }

        await result.WaitAsync(TimeSpan.FromSeconds(5));

        if (typedHandler)
            typedLegacy.Events.ShouldMatchReferences(new[] { (MessageEvent)callback.Event });
        else
            legacy.Callbacks.ShouldMatchReferences(new[] { callback });
    }

    record UserInputs
    {
        public string UserId { get; init; }
    }

    record CountInputs
    {
        public int Count { get; init; }
    }

    class ExternalEventContext(EventCallback callback, Event slackEvent, Func<Task> acknowledge) : Experimental.IEventContext<Event>
    {
        public EventCallback Callback => callback;
        public Event Event => slackEvent;
        public bool IsAcknowledged { get; private set; }

        public Task Acknowledge()
        {
            if (IsAcknowledged)
                throw new InvalidOperationException("Event already acknowledged");

            IsAcknowledged = true;
            return acknowledge();
        }
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