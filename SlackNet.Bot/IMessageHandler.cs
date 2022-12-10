using System.Threading.Tasks;

namespace SlackNet.Bot;

public interface IMessageHandler
{
    Task HandleMessage(IMessage message);
}