using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackMessageActionsService : ISlackMessageActions
    {
        private readonly ISlackMessageActions _actions = new SlackMessageActions();

        public SlackMessageActionsService(IEnumerable<ResolvedMessageActionHandler> handlers)
        {
            foreach (var handler in handlers)
                _actions.SetHandler(handler.CallbackId, handler);
        }

        public Task<MessageActionResponse> Handle(MessageAction request) => _actions.Handle(request);
        public void SetHandler(string callbackId, IMessageActionHandler handler) => _actions.SetHandler(callbackId, handler);
    }
}