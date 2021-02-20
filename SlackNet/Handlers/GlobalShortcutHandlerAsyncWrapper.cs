using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class GlobalShortcutHandlerAsyncWrapper : IAsyncGlobalShortcutHandler
    {
        private readonly IGlobalShortcutHandler _syncHandler;
        public GlobalShortcutHandlerAsyncWrapper(IGlobalShortcutHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(GlobalShortcut shortcut, Responder respond)
        {
            await _syncHandler.Handle(shortcut).ConfigureAwait(false);
            await respond().ConfigureAwait(false);
        }
    }
}