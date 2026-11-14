#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationKickResponse
{
    /// <summary>
    /// User IDs that were successfully removed.
    /// </summary>
    public IList<string> RemovedUserIds { get; set; } = [];

    /// <summary>
    /// Users that could not be removed, with error details.
    /// </summary>
    public IList<ConversationKickFailure> FailedUserIds { get; set; } = [];
}

/// <summary>
/// A user that could not be removed from a channel by <see cref="IAdminConversationsApi.Kick"/>.
/// </summary>
public class ConversationKickFailure
{
    public required string UserId { get; set; }

    /// <summary>
    /// The error code explaining why the removal failed.
    /// </summary>
    public required string Error { get; set; }
}
