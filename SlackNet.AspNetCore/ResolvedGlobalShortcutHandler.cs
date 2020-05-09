using System;
using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.AspNetCore
{
    class ResolvedGlobalShortcutHandler : ResolvedHandler<IGlobalShortcutHandler>, IGlobalShortcutHandler
    {
        public ResolvedGlobalShortcutHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IGlobalShortcutHandler> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task Handle(GlobalShortcut shortcut) => ResolvedHandle(h => h.Handle(shortcut));
    }
}