using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class CompositeMessageShortcutHandler : IMessageShortcutHandler
    {
        private readonly List<CompositeItem<IMessageShortcutHandler>> _handlers;
        public CompositeMessageShortcutHandler(IEnumerable<CompositeItem<IMessageShortcutHandler>> handlers) => _handlers = handlers.ToList();

        public Task Handle(MessageShortcut request) => Task.WhenAll(_handlers.Select(h => h.Item.Handle(request)));
    }
}