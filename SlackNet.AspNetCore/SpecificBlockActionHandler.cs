using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SpecificBlockActionHandler : IBlockActionHandler
    {
        private readonly string _actionId;
        private readonly IBlockActionHandler _handler;

        public SpecificBlockActionHandler(string actionId, IBlockActionHandler handler)
        {
            _actionId = actionId;
            _handler = handler;
        }

        public async Task Handle(BlockActionRequest request)
        {
            if (request.Action.ActionId == _actionId)
                await _handler.Handle(request).ConfigureAwait(false);
        }
    }
}