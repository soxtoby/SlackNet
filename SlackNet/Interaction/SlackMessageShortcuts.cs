using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface ISlackMessageShortcuts
    {
        Task Handle(MessageShortcut request);
        void AddHandler(IMessageShortcutHandler handler);
    }

    public class SlackMessageShortcuts : ISlackMessageShortcuts
    {
        private readonly List<IMessageShortcutHandler> _handlers = new List<IMessageShortcutHandler>();

        public Task Handle(MessageShortcut request) => Task.WhenAll(_handlers.Select(h => h.Handle(request)));

        public void AddHandler(IMessageShortcutHandler handler) => _handlers.Add(handler);
    }
}