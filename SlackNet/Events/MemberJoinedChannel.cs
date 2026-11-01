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
    /// Team ID, identifying which workspace the user is from.
    /// </summary>
    public string Team { get; set; }
    /// <summary>
    /// If the user was invited, the user ID of the inviting user.
    /// </summary>
    public string Inviter { get; set; }
    /// <summary>
    /// Enterprise ID, identifying which org the user is from. The value will be blank if the user's team is not an Enterprise organization.
    /// </summary>
    public string Enterprise { get; set; }
}