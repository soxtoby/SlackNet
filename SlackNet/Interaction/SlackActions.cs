using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackActions
    {
        Task<MessageResponse> Handle(InteractiveMessage request);
        void SetHandler(string actionName, IActionHandler handler);
    }

    public class SlackActions : ISlackActions
    {
        private readonly Dictionary<string, IActionHandler> _handlers = new Dictionary<string, IActionHandler>();

        public Task<MessageResponse> Handle(InteractiveMessage request) =>
            _handlers.TryGetValue(request.Actions[0].Name, out var handler)
                ? handler.Handle(request)
                : Task.FromResult<MessageResponse>(null);

        public void SetHandler(string actionName, IActionHandler handler) => _handlers[actionName] = handler;
    }
}