using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class BlockActionHandlerAsyncWrapper : IAsyncBlockActionHandler
    {
        private readonly IBlockActionHandler _syncHandler;
        public BlockActionHandlerAsyncWrapper(IBlockActionHandler syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(BlockActionRequest request, Responder respond)
        {
            await _syncHandler.Handle(request).ConfigureAwait(false);
            await respond().ConfigureAwait(false);
        }
    }

    class BlockActionHandlerAsyncWrapper<TAction> : IAsyncBlockActionHandler<TAction>
        where TAction : BlockAction
    {
        private readonly IBlockActionHandler<TAction> _syncHandler;
        public BlockActionHandlerAsyncWrapper(IBlockActionHandler<TAction> syncHandler) => _syncHandler = syncHandler;

        public async Task Handle(TAction action, BlockActionRequest request, Responder respond)
        {
            await _syncHandler.Handle(action, request).ConfigureAwait(false);
            await respond().ConfigureAwait(false);
        }
    }
}