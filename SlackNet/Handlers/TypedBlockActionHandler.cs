using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class TypedBlockActionHandler<TAction> : IBlockActionHandler, IComposedHandler<BlockActionRequest> where TAction : BlockAction
    {
        private readonly IBlockActionHandler<TAction> _handler;
        public TypedBlockActionHandler(IBlockActionHandler<TAction> handler) => _handler = handler;

        public Task Handle(BlockActionRequest request) =>
            request.Action is TAction blockAction
                ? _handler.Handle(blockAction, request)
                : Task.CompletedTask;

        IEnumerable<object> IComposedHandler<BlockActionRequest>.InnerHandlers(BlockActionRequest request) =>
            request.Action is TAction
                ? _handler.InnerHandlers(request)
                : Enumerable.Empty<object>();
    }
}