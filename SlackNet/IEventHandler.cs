using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet;

public interface IEventHandler
{
    Task Handle(EventCallback eventCallback);
}

public interface IEventHandler<in T> where T: Event
{
    Task Handle(T slackEvent);

    Task HandleWithContext(T slackEvent, EventContext context)
    {
        return Task.CompletedTask;
    }
}