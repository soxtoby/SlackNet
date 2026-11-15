using System;

namespace SlackNet.SocketMode;

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class Disconnect : SocketMessage
{
    public DisconnectReason Reason { get; set; }
    public DisconnectDebugInfo DebugInfo { get; set; }
}

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public enum DisconnectReason
{
    RefreshRequested,
    SocketModeDisabled
}

[Obsolete("Socket mode messages are handled by the socket mode client from SlackServiceBuilder or AddSlackNet. This will become internal in a future version.")]
public class DisconnectDebugInfo
{
    public string Host { get; set; }
}