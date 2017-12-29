using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.AspNetCore
{
    class SlackActionsService : ISlackActions
    {
        private readonly ISlackActions _actions = new SlackActions();

        public SlackActionsService(IEnumerable<ResolvedActionHandler> handlers)
        {
            foreach (var handler in handlers)
                _actions.SetHandler(handler.ActionName, handler);
        }

        public Task<MessageResponse> Handle(InteractiveMessage request) => _actions.Handle(request);
        public void SetHandler(string actionName, IActionHandler handler) => _actions.SetHandler(actionName, handler);
    }
}