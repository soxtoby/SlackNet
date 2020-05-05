using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.AspNetCore
{
    class CompositeEventHandler : IEventHandler
    {
        private readonly IEnumerable<CompositeItem<IEventHandler>> _handlers;
        public CompositeEventHandler(IEnumerable<CompositeItem<IEventHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(EventCallback eventCallback) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(eventCallback)));
    }
}