using System;
using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.AspNetCore
{
    class SlackEventsService : ISlackEvents
    {
        private readonly ISlackEvents _events = new SlackEvents();

        public SlackEventsService(IEnumerable<IEventHandler> eventHandlers)
        {
            foreach (var handler in eventHandlers)
                AddHandler((dynamic)handler);
        }

        public void Handle(EventCallback eventCallback) => _events.Handle(eventCallback);

        public IObservable<EventCallback> RawEvents => _events.RawEvents;

        public IObservable<T> Events<T>() where T : Event => _events.Events<T>();

        public void AddHandler<T>(IEventHandler<T> handler) where T : Event => _events.AddHandler(handler);
    }
}