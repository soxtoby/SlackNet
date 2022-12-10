namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a team channel is unarchived.
/// </summary>
public class ChannelUnarchive : Event
{
    public string Channel { get; set; }
    public string User { get; set; }
}