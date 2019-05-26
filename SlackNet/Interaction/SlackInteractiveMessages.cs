using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackInteractiveMessages
    {
        Task<MessageResponse> Handle(InteractiveMessage request);
        void SetHandler(string actionName, IInteractiveMessageHandler handler);
    }

    public class SlackInteractiveMessages : ISlackInteractiveMessages
    {
        private readonly Dictionary<string, IInteractiveMessageHandler> _handlers = new Dictionary<string, IInteractiveMessageHandler>();

        public Task<MessageResponse> Handle(InteractiveMessage request) =>
            _handlers.TryGetValue(request.Actions[0].Name, out var handler)
                ? handler.Handle(request)
                : Task.FromResult<MessageResponse>(null);

        public void SetHandler(string actionName, IInteractiveMessageHandler handler) => _handlers[actionName] = handler;
    }
}