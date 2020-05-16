using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class MessageActionAdapter : IMessageShortcutHandler
    {
        private readonly IMessageActionHandler _actionHandler;
        public MessageActionAdapter(IMessageActionHandler actionHandler) => _actionHandler = actionHandler;

        public Task Handle(MessageShortcut request) => _actionHandler.Handle(request);
    }
}