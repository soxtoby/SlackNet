using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
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
}