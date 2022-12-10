using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

class SpecificBlockActionHandler : IAsyncBlockActionHandler, IComposedHandler<BlockActionRequest>
{
    private readonly string _actionId;
    private readonly IAsyncBlockActionHandler _handler;

    public SpecificBlockActionHandler(string actionId, IAsyncBlockActionHandler handler)
    {
        _actionId = actionId;
        _handler = handler;
    }

    public async Task Handle(BlockActionRequest request, Responder respond)
    {
        if (request.Action.ActionId == _actionId)
            await _handler.Handle(request, respond).ConfigureAwait(false);
    }

    IEnumerable<object> IComposedHandler<BlockActionRequest>.InnerHandlers(BlockActionRequest request) =>
        request.Action.ActionId == _actionId
            ? _handler.InnerHandlers(request)
            : Enumerable.Empty<object>();
}