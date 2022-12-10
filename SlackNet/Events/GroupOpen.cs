namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a user when a private group is opened by that user.
/// </summary>
public class GroupOpen : Event
{
    public string User { get; set; }
    public string Channel { get; set; }
}