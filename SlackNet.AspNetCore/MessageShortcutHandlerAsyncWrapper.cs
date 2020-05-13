using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class MessageShortcutHandlerAsyncWrapper : IAsyncMessageShortcutHandler
    {
        private readonly IMessageShortcutHandler _syncHandler;
        public MessageShortcutHandlerAsyncWrapper(IMessageShortcutHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(MessageShortcut request, Responder respond)
        {
            await _syncHandler.Handle(request).ConfigureAwait(false);
            await respond().ConfigureAwait(false);
        }
    }
}