using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers;

class EventHandlerAsyncWrapper(IEventHandler handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
{
    public async Task Handle(Experimental.IEventContext<Event> eventContext)
    {
        await Acknowledge(eventContext).ConfigureAwait(false);
        await handler.Handle(eventContext.Callback).ConfigureAwait(false);
    }

    internal static Task Acknowledge(Experimental.IEventContext<Event> eventContext) =>
        eventContext is EventContext context
            ? context.AckIfNeeded()
            : eventContext.IsAcknowledged ? Task.CompletedTask : eventContext.Acknowledge();

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) => handler.InnerHandlers(eventCallback);
}

class EventHandlerAsyncWrapper<TEvent>(IEventHandler<TEvent> handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
    where TEvent : Event
{
    public async Task Handle(Experimental.IEventContext<Event> eventContext)
    {
        if (eventContext.Event is TEvent slackEvent)
        {
            await EventHandlerAsyncWrapper.Acknowledge(eventContext).ConfigureAwait(false);
            await handler.Handle(slackEvent).ConfigureAwait(false);
        }
    }

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        eventCallback.Event is TEvent
            ? handler.InnerHandlers(eventCallback)
            : Enumerable.Empty<object>();
}
