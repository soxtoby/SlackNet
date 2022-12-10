namespace SlackNet.Events;

/// <summary>
/// Sent to you when you have been removed to an existing User Group.
/// </summary>
public class SubteamSelfRemoved : Event
{
    public string SubteamId { get; set; }
}