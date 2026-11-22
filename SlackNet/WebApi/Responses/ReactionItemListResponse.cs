using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ReactionItemListResponse
{
    public IList<ReactionItem> Items { get; set; } = [];
    public Paging Paging { get; set; } = new();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}