using System.Threading.Tasks;

namespace SlackNet
{
    public interface IActionHandler
    {
        Task<MessageResponse> Handle(InteractiveMessage message);
    }
}