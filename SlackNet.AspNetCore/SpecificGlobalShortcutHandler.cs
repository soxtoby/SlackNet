using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SpecificGlobalShortcutHandler : IAsyncGlobalShortcutHandler
    {
        private readonly string _callbackId;
        private readonly IAsyncGlobalShortcutHandler _handler;

        public SpecificGlobalShortcutHandler(string callbackId, IAsyncGlobalShortcutHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(GlobalShortcut request, Responder respond)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request, respond).ConfigureAwait(false);
        }
    }
}