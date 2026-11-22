using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationBulkActionResponse
{
    public required string BulkActionId { get; set; }

    /// <summary>
    /// The channels that could not be added to the bulk action, and why.
    /// </summary>
    public IList<ConversationBulkActionFailure> NotAdded { get; set; } = [];
}

/// <summary>
/// A channel that could not be added to or updated by a bulk action, and why.
/// </summary>
public class ConversationBulkActionFailure
{
    public required string ChannelId { get; set; }

    /// <summary>
    /// The reasons the channel was not added or updated.
    /// </summary>
    public IList<string> Errors { get; set; } = [];
}
