using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackMessageActionsService : ISlackMessageActions
    {
        private readonly ISlackMessageActions _actions = new SlackMessageActions();

        public SlackMessageActionsService(IEnumerable<IMessageActionHandler> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);
        }

        public Task Handle(MessageAction request) => _actions.Handle(request);
        public void AddHandler(IMessageActionHandler handler) => _actions.AddHandler(handler);
    }
}