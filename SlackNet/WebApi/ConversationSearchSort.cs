namespace SlackNet.WebApi;

/// <summary>
/// How to sort the channels found by <see cref="IAdminConversationsApi.Search"/>.
/// </summary>
public enum ConversationSearchSort
{
    /// <summary>
    /// Search ranking based on what Slack thinks is closest.
    /// </summary>
    Relevant,

    /// <summary>
    /// Alphabetical.
    /// </summary>
    Name,

    /// <summary>
    /// Number of users in the channel.
    /// </summary>
    MemberCount,

    /// <summary>
    /// Date the channel was created.
    /// </summary>
    Created
}
