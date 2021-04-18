using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class GlobalShortcutHandlerAsyncWrapper : IAsyncGlobalShortcutHandler
    {
        private readonly IGlobalShortcutHandler _syncHandler;
        public GlobalShortcutHandlerAsyncWrapper(IGlobalShortcutHandler syncHandler) => _syncHandler = syncHandler;

        public Task Handle(GlobalShortcut shortcut, Responder respond) => _syncHandler.Handle(shortcut);
    }
}