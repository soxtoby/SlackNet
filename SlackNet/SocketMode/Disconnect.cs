namespace SlackNet.SocketMode
{
    public class Disconnect : SocketMessage
    {
        public DisconnectReason Reason { get; set; }
        public DisconnectDebugInfo DebugInfo { get; set; }
    }

    public enum DisconnectReason
    {
        RefreshRequested,
        SocketModeDisabled
    }

    public class DisconnectDebugInfo
    {
        public string Host { get; set; }
    }
}