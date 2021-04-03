using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class SpecificGlobalShortcutHandler : IAsyncGlobalShortcutHandler, IComposedHandler<GlobalShortcut>
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

        IEnumerable<object> IComposedHandler<GlobalShortcut>.InnerHandlers(GlobalShortcut request) =>
            request.CallbackId == _callbackId
                ? _handler.InnerHandlers(request)
                : Enumerable.Empty<object>();
    }
}