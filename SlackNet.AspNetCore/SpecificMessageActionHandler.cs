using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    public class SpecificMessageActionHandler : IMessageActionHandler
    {
        private readonly string _callbackId;
        private readonly IMessageActionHandler _handler;

        public SpecificMessageActionHandler(string callbackId, IMessageActionHandler handler)
        {
            _callbackId = callbackId;
            _handler = handler;
        }

        public async Task Handle(MessageAction request)
        {
            if (request.CallbackId == _callbackId)
                await _handler.Handle(request).ConfigureAwait(false);
        }
    }
}