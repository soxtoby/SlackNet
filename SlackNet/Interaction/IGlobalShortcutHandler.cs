using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IGlobalShortcutHandler
    {
        Task Handle(GlobalShortcut shortcut);
    }
}