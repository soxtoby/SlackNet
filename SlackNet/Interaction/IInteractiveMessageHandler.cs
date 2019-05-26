using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IInteractiveMessageHandler
    {
        Task<MessageResponse> Handle(InteractiveMessage message);
    }
}