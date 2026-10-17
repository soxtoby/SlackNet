using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers;

class CompositeContextEventHandler(IEnumerable<Experimental.IEventHandler<Event>> handlers)
    : Experimental.IEventHandler<Event>, IComposedHandler<EventCallback>
{
    public Task Handle(Experimental.IEventContext<Event> eventContext) => Task.WhenAll(handlers.Select(h => h.Handle(eventContext)));

    IEnumerable<object> IComposedHandler<EventCallback>.InnerHandlers(EventCallback callback) => handlers.SelectMany(h => h.InnerHandlers(callback));
}
