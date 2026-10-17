using System.Threading.Tasks;
using SlackNet.Events;

namespace SlackNet.Handlers.Experimental;

/// <summary>
/// Handles events with access to their metadata and control over acknowledgement.
/// Events are acknowledged automatically when handling completes without calling <see cref="IEventContext{T}.Acknowledge"/>.
/// </summary>
public interface IEventHandler<in T> where T : Event
{
    Task Handle(IEventContext<T> eventContext);
}