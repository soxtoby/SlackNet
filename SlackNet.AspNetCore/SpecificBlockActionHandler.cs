using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SpecificBlockActionHandler<TAction> : IBlockActionHandler<TAction> 
        where TAction : BlockAction
    {
        private readonly string _actionId;
        private readonly IBlockActionHandler<TAction> _handler;

        public SpecificBlockActionHandler(string actionId, IBlockActionHandler<TAction> handler)
        {
            _actionId = actionId;
            _handler = handler;
        }

        public async Task Handle(TAction action, BlockActionRequest request)
        {
            if (request.Action.ActionId == _actionId)
                await _handler.Handle(action, request).ConfigureAwait(false);
        }
    }
}