namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a team channel is deleted.
/// </summary>
public class ChannelDeleted : Event
{
    public string Channel { get; set; }
}