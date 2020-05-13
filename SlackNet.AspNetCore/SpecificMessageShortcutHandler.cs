using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SpecificMessageShortcutHandler : IAsyncMessageShortcutHandler
    {
        private readonly string _callbackId;
        private readonly IAsyncMessageShortcutHandler _handler;

        public SpecificMessageShortcutHandler(string callbackId, IAsyncMessageShortcutHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(MessageShortcut request, Responder respond)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request, respond).ConfigureAwait(false);
        }
    }
}