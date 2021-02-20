using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class CompositeGlobalShortcutHandler : IAsyncGlobalShortcutHandler
    {
        private readonly IEnumerable<IAsyncGlobalShortcutHandler> _handlers;
        public CompositeGlobalShortcutHandler(IEnumerable<IAsyncGlobalShortcutHandler> handlers) => _handlers = handlers;

        public Task Handle(GlobalShortcut shortcut, Responder respond) => Task.WhenAll(_handlers.Select(h => h.Handle(shortcut, respond)));
    }
}