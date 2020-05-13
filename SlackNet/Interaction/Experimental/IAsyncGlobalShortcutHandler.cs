using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental
{
    [Obsolete(Warning.Experimental)]
    public interface IAsyncGlobalShortcutHandler
    {
        Task Handle(GlobalShortcut shortcut, Responder respond);
    }
}