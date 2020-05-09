using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class CompositeGlobalShortcutHandler : IGlobalShortcutHandler
    {
        private readonly List<CompositeItem<IGlobalShortcutHandler>> _handlers;
        public CompositeGlobalShortcutHandler(IEnumerable<CompositeItem<IGlobalShortcutHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(GlobalShortcut shortcut) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(shortcut)));
    }
}