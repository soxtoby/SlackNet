using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers.Experimental;

public interface IEventContext<out T> where T : Event
{
    /// <summary>
    /// Also referred to as the "outer event". Metadata about the event.
    /// </summary>
    EventCallback Callback { get; }

    /// <summary>
    /// The event that was received from Slack. Same as <see cref="EventCallback.Event"/>.
    /// </summary>
    T Event { get; }

    /// <summary>
    /// Lets Slack know that the event has been received and processed successfully.
    /// Will throw if the event has already been acknowledged.
    /// </summary>
    Task Acknowledge();

    /// <summary>
    /// Indicates whether the event has been acknowledged. If true, calling <see cref="Acknowledge"/> will throw an exception.
    /// </summary>
    bool IsAcknowledged { get; }
}
