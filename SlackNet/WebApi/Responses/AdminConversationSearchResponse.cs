#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AdminConversationSearchResponse
{
    public IList<AdminConversation> Conversations { get; set; } = [];

    /// <summary>
    /// Encoded string for the next cursor value.
    /// </summary>
    public string NextCursor { get; set; } = string.Empty;

    /// <summary>
    /// Total number of channels that exist in the search index and match the query string.
    /// </summary>
    public int TotalCount { get; set; }
}
