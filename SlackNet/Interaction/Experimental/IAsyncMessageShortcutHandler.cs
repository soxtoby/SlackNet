using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction.Experimental
{
    [Obsolete(Warning.Experimental)]
    public interface IAsyncMessageShortcutHandler
    {
        Task Handle(MessageShortcut request, Responder respond);
    }
}