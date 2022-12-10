namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a user when a direct message channel is opened by that user.
/// </summary>
public class ImOpen : Event
{
    public string User { get; set; }
    public string Channel { get; set; }
}