using System;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet;

/// <summary>
/// Provides an event, its callback metadata, and a single acknowledgement shared by its handlers.
/// </summary>
public class EventContext(EventCallback callback, Func<Task> acknowledge) : Handlers.Experimental.IEventContext<Event>
{
    private readonly Lazy<Task> _acknowledgement = new(acknowledge);
    private int _isAcknowledged;

    public EventCallback Callback { get; } = callback;
    public Event Event => Callback.Event;
    public bool IsAcknowledged => Volatile.Read(ref _isAcknowledged) != 0;

    public Task Acknowledge()
    {
        return Interlocked.Exchange(ref _isAcknowledged, 1) == 0
            ? _acknowledgement.Value
            : throw new InvalidOperationException("Event already acknowledged");
    }

    internal Task AckIfNeeded()
    {
        Interlocked.Exchange(ref _isAcknowledged, 1);
        return _acknowledgement.Value;
    }
}
