using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.AspNetCore
{
    public interface IEventsObservables : IEventHandler
    {
        IObservable<EventCallback> RawEvents { get; }
        IObservable<TEvent> Events<TEvent>();
    }

    class EventsObservables : IEventsObservables, IDisposable
    {
        private readonly SyncedSubject<EventCallback> _incomingEvents = new SyncedSubject<EventCallback>();

        public Task Handle(EventCallback eventCallback)
        {
            _incomingEvents.OnNext(eventCallback);
            return Task.CompletedTask;
        }

        public IObservable<EventCallback> RawEvents => _incomingEvents.AsObservable();

        public IObservable<TEvent> Events<TEvent>() => _incomingEvents
            .Select(e => e.Event)
            .OfType<TEvent>();

        public void Dispose() => _incomingEvents.Dispose();
    }
}