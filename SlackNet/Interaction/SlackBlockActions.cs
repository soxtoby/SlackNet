using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public interface ISlackBlockActions
    {
        Task Handle(BlockActionRequest request);
        void AddHandler<TAction>(IBlockActionHandler<TAction> handler) where TAction : BlockAction;
    }

    public class SlackBlockActions : ISlackBlockActions
    {
        private readonly List<IBlockActionHandler> _handlers = new List<IBlockActionHandler>();

        public Task Handle(BlockActionRequest request) => HandleGeneric((dynamic)request.Action, request);

        private Task HandleGeneric<TAction>(TAction action, BlockActionRequest request) 
            where TAction : BlockAction
        {
            return Task.WhenAll(_handlers
                .OfType<IBlockActionHandler<TAction>>()
                .Select(h => h.Handle(action, request)));
        }

        public void AddHandler<TAction>(IBlockActionHandler<TAction> handler) where TAction : BlockAction => _handlers.Add(handler);
    }
}