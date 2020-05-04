using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public class SpecificMessageShortcutHandler : IMessageShortcutHandler
    {
        private readonly string _callbackId;
        private readonly IMessageShortcutHandler _handler;

        public SpecificMessageShortcutHandler(string callbackId, IMessageShortcutHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(MessageShortcut request)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request).ConfigureAwait(false);
        }
    }
}