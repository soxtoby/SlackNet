using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.Handlers
{
    public class SwitchingInteractiveMessageHandler : IInteractiveMessageHandler
    {
        private readonly IHandlerIndex<IInteractiveMessageHandler> _handlers;
        public SwitchingInteractiveMessageHandler(IHandlerIndex<IInteractiveMessageHandler> handlers) => _handlers = handlers;

        public Task<MessageResponse> Handle(InteractiveMessage message) =>
            _handlers.TryGetValue(message.Actions[0].Name, out var handler)
                ? handler.Handle(message)
                : Task.FromResult<MessageResponse>(null);
    }
}