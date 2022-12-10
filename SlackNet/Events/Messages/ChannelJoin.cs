namespace SlackNet.Events;

/// <summary>
/// Sometimes sent to all connections for a user when that user leaves a channel. It is sometimes withheld.
/// </summary>
public class ChannelJoin : MessageEvent
{
    /// <summary>
    /// If the user was invited, the user ID of the inviting user.
    /// </summary>
    public string Inviter { get; set; }
}