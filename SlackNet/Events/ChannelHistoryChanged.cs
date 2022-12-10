namespace SlackNet.Events;

/// <summary>
/// Sent to all clients in a channel when bulk changes have occurred to that channel's history.
/// </summary>
public class ChannelHistoryChanged : Event
{
    /// <summary>
    /// Id of latest changed message.
    /// </summary>
    public string Latest { get; set; }
    public string Ts { get; set; }
}