using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet
{
    public interface IEventHandler { }

    public interface IEventHandler<T> : IEventHandler where T: Event
    {
        Task Handle(T slackEvent);
    }
}