using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers;

class EventHandlerAsyncWrapper(IEventHandler handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
{
    public async Task Handle(Experimental.IEventContext<Event> eventContext)
    {
        if (!eventContext.IsAcknowledged)
            await eventContext.Acknowledge().ConfigureAwait(false);
        await handler.Handle(eventContext.Callback).ConfigureAwait(false);
    }

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) => handler.InnerHandlers(eventCallback);
}

class EventHandlerAsyncWrapper<TEvent>(IEventHandler<TEvent> handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
    where TEvent : Event
{
    public async Task Handle(Experimental.IEventContext<Event> eventContext)
    {
        if (eventContext.Event is TEvent slackEvent)
        {
            if (!eventContext.IsAcknowledged)
                await eventContext.Acknowledge().ConfigureAwait(false);
            await handler.Handle(slackEvent).ConfigureAwait(false);
        }
    }

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        eventCallback.Event is TEvent
            ? handler.InnerHandlers(eventCallback)
            : Enumerable.Empty<object>();
}
