using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class TypedBlockActionHandler<TAction> : IBlockActionHandler where TAction : BlockAction
    {
        private readonly IBlockActionHandler<TAction> _handler;
        public TypedBlockActionHandler(IBlockActionHandler<TAction> handler) => _handler = handler;

        public Task Handle(BlockActionRequest request) =>
            request.Action is TAction blockAction
                ? _handler.Handle(blockAction, request)
                : Task.CompletedTask;
    }
}