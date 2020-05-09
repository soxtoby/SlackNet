using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SpecificGlobalShortcutHandler : IGlobalShortcutHandler
    {
        private readonly string _callbackId;
        private readonly IGlobalShortcutHandler _handler;

        public SpecificGlobalShortcutHandler(string callbackId, IGlobalShortcutHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(GlobalShortcut request)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request).ConfigureAwait(false);
        }
    }
}