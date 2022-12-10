namespace SlackNet.Events;

/// <summary>
/// Sent to all open connections for a user when that user moves the read cursor in a channel.
/// </summary>
public class ChannelMarked : Event
{
    public string Channel { get; set; }
    public string Ts { get; set; }
}