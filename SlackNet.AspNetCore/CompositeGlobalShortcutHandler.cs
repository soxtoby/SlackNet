using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class CompositeGlobalShortcutHandler : IAsyncGlobalShortcutHandler
    {
        private readonly List<CompositeItem<IAsyncGlobalShortcutHandler>> _handlers;
        public CompositeGlobalShortcutHandler(IEnumerable<CompositeItem<IAsyncGlobalShortcutHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(GlobalShortcut shortcut, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(shortcut, respond)));
    }
}