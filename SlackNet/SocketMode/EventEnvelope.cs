using System;
using SlackNet.Events;

namespace SlackNet.SocketMode;

[SlackType("events_api")]
[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class EventEnvelope : SocketEnvelope<EventCallback>;