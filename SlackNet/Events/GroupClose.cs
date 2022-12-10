namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a user when a private channel is closed by that user.
/// </summary>
public class GroupClose : Event
{
    public string User { get; set; }
    public string Channel { get; set; }
}