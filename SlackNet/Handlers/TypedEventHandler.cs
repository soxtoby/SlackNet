using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers;

public class TypedEventHandler<TEvent> : IEventHandler, IComposedHandler<EventCallback> where TEvent : Event
{
    private readonly IEventHandler<TEvent> _eventHandler;
    public TypedEventHandler(IEventHandler<TEvent> eventHandler) => _eventHandler = eventHandler;

    public Task Handle(EventCallback eventCallback) => 
        eventCallback.Event is TEvent slackEvent
            ? Task.WhenAll(_eventHandler.Handle(slackEvent), _eventHandler.HandleWithContext(slackEvent, new EventContext(eventCallback)))
            : Task.CompletedTask;
    

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback eventCallback) =>
        eventCallback.Event is TEvent
            ? _eventHandler.InnerHandlers(eventCallback)
            : Enumerable.Empty<object>();
}