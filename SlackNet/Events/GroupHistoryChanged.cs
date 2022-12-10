namespace SlackNet.Events;

/// <summary>
/// Sent to all clients in a private channel when bulk changes have occurred to that group's history.
/// </summary>
public class GroupHistoryChanged : Event
{
    /// <summary>
    /// Id of latest changed message.
    /// </summary>
    public string Latest { get; set; }
    public string Ts { get; set; }
}