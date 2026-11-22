#nullable disable
namespace SlackNet;

/// <summary>
/// A conversation's retention policy, as returned by <see cref="WebApi.IAdminConversationsApi.GetCustomRetention"/>.
/// </summary>
public class ConversationRetention
{
    /// <summary>
    /// Is there a policy set on this conversation that overrides the default retention policy.
    /// </summary>
    public bool IsPolicyEnabled { get; set; }

    /// <summary>
    /// The message retention duration in days for this conversation.
    /// </summary>
    public int DurationDays { get; set; }
}
