using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class CompositeMessageShortcutHandler : IAsyncMessageShortcutHandler
    {
        private readonly List<CompositeItem<IAsyncMessageShortcutHandler>> _handlers;
        public CompositeMessageShortcutHandler(IEnumerable<CompositeItem<IAsyncMessageShortcutHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(MessageShortcut request, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(request, respond)));
    }
}