using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace SlackNet.Handlers;

class GlobalShortcutHandlerAsyncWrapper : IAsyncGlobalShortcutHandler, IComposedHandler<GlobalShortcut>
{
    private readonly IGlobalShortcutHandler _syncHandler;
    public GlobalShortcutHandlerAsyncWrapper(IGlobalShortcutHandler syncHandler) => _syncHandler = syncHandler;

    public Task Handle(GlobalShortcut shortcut, Responder respond) => _syncHandler.Handle(shortcut);

    IEnumerable<object> IComposedHandler<GlobalShortcut>.InnerHandlers(GlobalShortcut request) => _syncHandler.InnerHandlers(request);
}