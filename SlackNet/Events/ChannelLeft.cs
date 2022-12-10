namespace SlackNet.Events;

/// <summary>
/// Sometimes sent to all connections for a user when that user leaves a channel. It is sometimes withheld.
/// </summary>
public class ChannelLeft : Event
{
    public string Channel { get; set; }
}