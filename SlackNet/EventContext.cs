using System;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.Handlers.Experimental;

namespace SlackNet;

/// <summary>
/// The context created for an incoming event, owning its acknowledgement.
/// </summary>
public class EventContext(EventCallback callback, Func<Task> acknowledge) : IEventContext<Event>
{
    private int _isAcknowledged;

    public EventCallback Callback => callback;
    public Event Event => callback.Event;
    public bool IsAcknowledged => Volatile.Read(ref _isAcknowledged) != 0;

    public Task Acknowledge() =>
        Interlocked.Exchange(ref _isAcknowledged, 1) == 0
            ? acknowledge()
            : throw new InvalidOperationException("Event already acknowledged.");
}

/// <summary>
/// A typed view of another context, sharing its callback and acknowledgement.
/// </summary>
class EventContext<TEvent>(IEventContext<Event> context, TEvent slackEvent) : IEventContext<TEvent>
    where TEvent : Event
{
    public EventCallback Callback => context.Callback;
    public TEvent Event => slackEvent;
    public bool IsAcknowledged => context.IsAcknowledged;
    public Task Acknowledge() => context.Acknowledge();
}
