using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IActionHandler
    {
        Task<MessageResponse> Handle(InteractiveMessage message);
    }
}