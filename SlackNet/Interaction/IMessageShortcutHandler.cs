using System;
using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IMessageShortcutHandler
    {
        Task Handle(MessageShortcut request);
    }

    [Obsolete("Use IMessageShortcutHandler instead")]
    public interface IMessageActionHandler
    {
        Task Handle(MessageAction request);
    }
}