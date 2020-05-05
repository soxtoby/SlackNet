using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SwitchingInteractiveMessageHandler : IInteractiveMessageHandler
    {
        private readonly Dictionary<string, IInteractiveMessageHandler> _handlers;
        public SwitchingInteractiveMessageHandler(IEnumerable<KeyedItem<IInteractiveMessageHandler>> handlers) => 
            _handlers = handlers.ToDictionary(h => h.Key, h => h.Item);

        public Task<MessageResponse> Handle(InteractiveMessage message) =>
            _handlers.TryGetValue(message.Actions[0].Name, out var handler)
                ? handler.Handle(message)
                : Task.FromResult<MessageResponse>(null);
    }
}