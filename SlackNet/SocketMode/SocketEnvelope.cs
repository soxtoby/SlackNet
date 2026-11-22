#nullable disable
using System;

namespace SlackNet.SocketMode;

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public abstract class SocketEnvelope : SocketMessage
{
    public string EnvelopeId { get; set; }
    public bool AcceptsResponsePayload { get; set; }
    public int? RetryAttempt { get; set; }
    public string RetryReason { get; set; }
}

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public abstract class SocketEnvelope<T> : SocketEnvelope
{
    public T Payload { get; set; }
}