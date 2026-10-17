using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.Handlers.Experimental;

namespace SlackNet.Handlers;

class TypedContextEventHandler<TEvent>(Experimental.IEventHandler<TEvent> handler) : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
    where TEvent : Event
{
    public Task Handle(IEventContext<Event> eventContext) =>
        eventContext.Event is TEvent slackEvent
            ? handler.Handle(new TypedEventContext(eventContext, slackEvent))
            : Task.CompletedTask;

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        eventCallback.Event is TEvent
            ? handler.InnerHandlers(eventCallback)
            : Enumerable.Empty<object>();

    class TypedEventContext(IEventContext<Event> context, TEvent slackEvent) : IEventContext<TEvent>
    {
        public EventCallback Callback => context.Callback;
        public TEvent Event => slackEvent;
        public bool IsAcknowledged => context.IsAcknowledged;
        public Task Acknowledge() => context.Acknowledge();
    }
}
