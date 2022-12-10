namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a team preference is changed.
/// </summary>
public class TeamPrefChange : Event
{
    public string Name { get; set; }
    public object Value { get; set; }
}