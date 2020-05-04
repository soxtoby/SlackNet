using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class SlackMessageShortcutsService : ISlackMessageShortcuts
    {
        private readonly ISlackMessageShortcuts _shortcuts = new SlackMessageShortcuts();

        public SlackMessageShortcutsService(IEnumerable<IMessageShortcutHandler> handlers)
        {
            foreach (var handler in handlers)
                AddHandler(handler);
        }

        public Task Handle(MessageShortcut request) => _shortcuts.Handle(request);
        public void AddHandler(IMessageShortcutHandler handler) => _shortcuts.AddHandler(handler);
    }
}