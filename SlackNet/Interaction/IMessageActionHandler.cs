using System.Threading.Tasks;

namespace SlackNet.Interaction
{
    public interface IMessageActionHandler
    {
        Task Handle(MessageAction request);
    }
}