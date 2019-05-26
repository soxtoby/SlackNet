using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackInteractiveMessagesService : ISlackInteractiveMessages
    {
        private readonly ISlackInteractiveMessages _interactiveMessages = new SlackInteractiveMessages();

        public SlackInteractiveMessagesService(IEnumerable<ResolvedInteractiveMessageHandler> handlers)
        {
            foreach (var handler in handlers)
                _interactiveMessages.SetHandler(handler.ActionName, handler);
        }

        public Task<MessageResponse> Handle(InteractiveMessage request) => _interactiveMessages.Handle(request);
        public void SetHandler(string actionName, IInteractiveMessageHandler handler) => _interactiveMessages.SetHandler(actionName, handler);
    }
}