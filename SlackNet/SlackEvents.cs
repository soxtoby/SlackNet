using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet
{
    public interface ISlackEvents
    {
        void Handle(EventCallback eventCallback);
        IObservable<EventCallback> RawEvents { get; }
        IObservable<T> Events<T>() where T : Event;
        void AddHandler<T>(IEventHandler<T> handler) where T : Event;
    }

    public class SlackEvents : ISlackEvents, IDisposable
    {
        private readonly SyncedSubject<EventCallback> _incomingEvents = new SyncedSubject<EventCallback>();
        private readonly List<IEventHandler> _handlers = new List<IEventHandler>();

        public void Handle(EventCallback eventCallback)
        {
            HandleGeneric((dynamic)eventCallback.Event);
            _incomingEvents.OnNext(eventCallback);
        }

        private async Task HandleGeneric<T>(T slackEvent) where T : Event
        {
            foreach (var handler in _handlers.OfType<IEventHandler<T>>())
                await handler.Handle(slackEvent).ConfigureAwait(false);
        }

        public IObservable<EventCallback> RawEvents => _incomingEvents.AsObservable();

        public IObservable<T> Events<T>() where T : Event => _incomingEvents
            .Select(e => e.Event)
            .OfType<T>();

        public void AddHandler<T>(IEventHandler<T> handler) where T : Event => _handlers.Add(handler);

        public void Dispose()
        {
            _incomingEvents.Dispose();
        }
    }
}