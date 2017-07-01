using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class ReactionItemListResponse
    {
        public IList<ReactionItem> Items { get; set; }
        public Paging Paging { get; set; }
    }
}