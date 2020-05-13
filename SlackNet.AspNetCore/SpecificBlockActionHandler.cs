using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class SpecificBlockActionHandler : IAsyncBlockActionHandler
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
    }
}