using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class BlockActionHandlerAsyncWrapper : IAsyncBlockActionHandler
    {
        private readonly IBlockActionHandler _syncHandler;
        public BlockActionHandlerAsyncWrapper(IBlockActionHandler syncHandler) => _syncHandler = syncHandler;

        public Task Handle(BlockActionRequest request, Responder respond) => _syncHandler.Handle(request);
    }
}