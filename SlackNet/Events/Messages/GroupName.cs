namespace SlackNet.Events;

/// <summary>
/// Sent when a private group is renamed.
/// </summary>
public class GroupName : MessageEvent
{
    public string OldName { get; set; }
    public string Name { get; set; }
}