using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers
{
    public class CompositeEventHandler : IEventHandler
    {
        private readonly IEnumerable<IEventHandler> _handlers;
        public CompositeEventHandler(IEnumerable<IEventHandler> handlers) => _handlers = handlers;

        public Task Handle(EventCallback eventCallback) => Task.WhenAll(_handlers.Select(h => h.Handle(eventCallback)));
    }
}