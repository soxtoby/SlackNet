using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackMessageActions
    {
        Task<MessageActionResponse> Handle(MessageAction request);
        void SetHandler(string callbackId, IMessageActionHandler handler);
    }

    public class SlackMessageActions : ISlackMessageActions
    {
        private readonly Dictionary<string, IMessageActionHandler> _handlers = new Dictionary<string, IMessageActionHandler>();

        public Task<MessageActionResponse> Handle(MessageAction request) =>
            _handlers.TryGetValue(request.CallbackId, out var handler)
                ? handler.Handle(request)
                : Task.FromResult<MessageActionResponse>(null);

        public void SetHandler(string callbackId, IMessageActionHandler handler) => _handlers[callbackId] = handler;
    }
}