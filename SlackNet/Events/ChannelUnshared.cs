namespace SlackNet.Events;

/// <summary>
/// Sent to all event subscriptions when an external workspace has been removed from an existing shared channel.
/// </summary>
public class ChannelUnshared : Event
{
    /// <summary>
    /// The team ID of the workspace that has been removed from the channel.
    /// </summary>
    public string PreviouslyConnectedTeamId { get; set; }
    /// <summary>
    /// The ID for the public or private channel.
    /// </summary>
    public string Channel { get; set; }
    /// <summary>
    /// True if the channel is still externally shared, and False otherwise.
    /// </summary>
    public bool IsExtShared { get; set; }
}