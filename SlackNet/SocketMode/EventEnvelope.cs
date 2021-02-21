using SlackNet.Events;

namespace SlackNet.SocketMode
{
    [SlackType("events_api")]
    public class EventEnvelope : SocketEnvelope<EventCallback> { }
}