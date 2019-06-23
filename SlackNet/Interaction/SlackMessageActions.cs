using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackMessageActions
    {
        Task Handle(MessageAction request);
        void AddHandler(IMessageActionHandler handler);
    }

    public class SlackMessageActions : ISlackMessageActions
    {
        private readonly List<IMessageActionHandler> _handlers = new List<IMessageActionHandler>();

        public Task Handle(MessageAction request) => Task.WhenAll(_handlers.Select(h => h.Handle(request)));

        public void AddHandler(IMessageActionHandler handler) => _handlers.Add(handler);
    }
}