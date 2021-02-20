using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers
{
    public class TypedAsyncBlockActionHandler<TAction> : IAsyncBlockActionHandler where TAction : BlockAction
    {
        private readonly IAsyncBlockActionHandler<TAction> _handler;
        public TypedAsyncBlockActionHandler(IAsyncBlockActionHandler<TAction> handler) => _handler = handler;

        public Task Handle(BlockActionRequest request, Responder respond) =>
            request.Action is TAction blockAction
                ? _handler.Handle(blockAction, request, respond)
                : Task.CompletedTask;
    }
}