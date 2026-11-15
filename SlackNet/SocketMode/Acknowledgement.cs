using System;

namespace SlackNet.SocketMode;

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class Acknowledgement
{
    public string EnvelopeId { get; set; }
}

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class Acknowledgement<T> : Acknowledgement
{
    public T Payload { get; set; }
}