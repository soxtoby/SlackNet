namespace SlackNet.Events;

/// <summary>
/// A call was rejected.
/// If a call has been shared with a user in DM (and unfurled with the help of the <see cref="LinkShared"/> event) this event is sent if the user rejects the call.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/call_rejected/">Slack documentation</a> for more information.</remarks>
public class CallRejected : Event
{
    /// <summary>
    /// The <see cref="Call.Id"/> of the call that was rejected.
    /// </summary>
    public string CallId { get; set; }

    /// <summary>
    /// The <see cref="User.Id"/> of the user who rejected the call.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The <see cref="Conversation.Id"/> of the DM the call was shared in.
    /// </summary>
    public string ChannelId { get; set; }

    /// <summary>
    /// The <see cref="Call.ExternalUniqueId"/> of the call that was rejected.
    /// </summary>
    public string ExternalUniqueId { get; set; }
}
