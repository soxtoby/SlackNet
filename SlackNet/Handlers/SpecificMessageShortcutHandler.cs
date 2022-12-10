using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

class SpecificMessageShortcutHandler : IAsyncMessageShortcutHandler, IComposedHandler<MessageShortcut>
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

    IEnumerable<object> IComposedHandler<MessageShortcut>.InnerHandlers(MessageShortcut request) =>
        request.CallbackId == _callbackId
            ? _handler.InnerHandlers(request)
            : Enumerable.Empty<object>();
}