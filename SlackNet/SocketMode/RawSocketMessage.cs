namespace SlackNet.SocketMode;

public class RawSocketMessage
{
    /// <summary>
    /// SlackNet-specific property identifying which web socket the message came from.
    /// Responses need to be sent back on the same web socket connection.
    /// </summary>
    public int SocketId { get; set; }

    /// <summary>
    /// SlackNet-specific property identifying the request from Slack, for correlating log events.
    /// </summary>
    public string RequestId { get; set; }

    public string Message { get; set; }
}