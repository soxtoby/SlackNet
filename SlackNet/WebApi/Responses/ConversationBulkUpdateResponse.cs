using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationBulkUpdateResponse
{
    public required string BulkActionId { get; set; }

    /// <summary>
    /// Channels that could not be updated, with reasons.
    /// </summary>
    public IList<ConversationBulkActionFailure> NotUpdated { get; set; } = [];
}
