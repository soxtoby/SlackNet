namespace SlackNet.Events;

/// <summary>
/// Sent to you when you have been added to an existing User Group.
/// </summary>
public class SubteamSelfAdded : Event
{
    public string SubteamId { get; set; }
}