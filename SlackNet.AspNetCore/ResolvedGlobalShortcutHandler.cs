using System;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.AspNetCore
{
    class ResolvedGlobalShortcutHandler : ResolvedHandler<IAsyncGlobalShortcutHandler>, IAsyncGlobalShortcutHandler
    {
        public ResolvedGlobalShortcutHandler(IServiceProvider serviceProvider, Func<IServiceProvider, IAsyncGlobalShortcutHandler> getHandler)
            : base(serviceProvider, getHandler) { }

        public Task Handle(GlobalShortcut shortcut, Responder respond) => ResolvedHandle(h => h.Handle(shortcut, respond));
    }
}