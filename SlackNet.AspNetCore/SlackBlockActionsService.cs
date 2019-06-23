using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackBlockActionsService : ISlackBlockActions
    {
        private readonly SlackBlockActions _slackBlockActions = new SlackBlockActions();

        public SlackBlockActionsService(IEnumerable<IBlockActionHandler> handlers)
        {
            foreach (var handler in handlers)
                AddHandler((dynamic)handler);
        }

        public Task Handle(BlockActionRequest request) => _slackBlockActions.Handle(request);

        public void AddHandler<TAction>(IBlockActionHandler<TAction> handler) where TAction : BlockAction => _slackBlockActions.AddHandler(handler);
    }
}