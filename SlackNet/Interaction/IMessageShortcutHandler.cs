using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IMessageShortcutHandler
    {
        Task Handle(MessageShortcut request);
    }
}