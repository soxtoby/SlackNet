using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class MessageShortcutHandlerAsyncWrapper : IAsyncMessageShortcutHandler
    {
        private readonly IMessageShortcutHandler _syncHandler;
        public MessageShortcutHandlerAsyncWrapper(IMessageShortcutHandler syncHandler) => _syncHandler = syncHandler;

        public Task Handle(MessageShortcut request, Responder respond) => _syncHandler.Handle(request);
    }
}