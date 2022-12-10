namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a user when a user preference is changed. 
/// </summary>
public class PrefChange : Event
{
    public string Name { get; set; }
    public string Value { get; set; }
}