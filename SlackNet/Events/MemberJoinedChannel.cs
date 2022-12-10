namespace SlackNet.Events;

/// <summary>
/// Sent to all websocket connections and event subscriptions when users join public or private channels.
/// </summary>
public class MemberJoinedChannel : Event
{
    public string User { get; set; }
    public string Channel { get; set; }
    public ChannelType ChannelType { get; set; }
    /// <summary>
    /// If the user was invited, the user ID of the inviting user.
    /// </summary>
    public string Inviter { get; set; }
}