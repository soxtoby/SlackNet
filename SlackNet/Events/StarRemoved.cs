namespace SlackNet.Events;

/// <summary>
/// Sent to all connected clients for users who can see the content that was starred when an item is starred.
/// </summary>
public class StarRemoved : Event
{
    public string User { get; set; }
    public StarredItem Item { get; set; }
}