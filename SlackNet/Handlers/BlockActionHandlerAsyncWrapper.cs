using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    class BlockActionHandlerAsyncWrapper : IAsyncBlockActionHandler, IComposedHandler<BlockActionRequest>
    {
        private readonly IBlockActionHandler _syncHandler;
        public BlockActionHandlerAsyncWrapper(IBlockActionHandler syncHandler) => _syncHandler = syncHandler;

        public Task Handle(BlockActionRequest request, Responder respond) => _syncHandler.Handle(request);

        IEnumerable<object> IComposedHandler<BlockActionRequest>.InnerHandlers(BlockActionRequest request) => _syncHandler.InnerHandlers(request);
    }
}