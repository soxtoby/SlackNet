using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers
{
    public class TypedEventHandler<TEvent> : IEventHandler where TEvent : Event
    {
        private readonly IEventHandler<TEvent> _eventHandler;
        public TypedEventHandler(IEventHandler<TEvent> eventHandler) => _eventHandler = eventHandler;

        public Task Handle(EventCallback eventCallback) =>
            eventCallback.Event is TEvent slackEvent
                ? _eventHandler.Handle(slackEvent)
                : Task.CompletedTask;
    }
}