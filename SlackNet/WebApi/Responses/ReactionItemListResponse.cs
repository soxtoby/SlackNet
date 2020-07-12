using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class ReactionItemListResponse
    {
        public IList<ReactionItem> Items { get; set; } = new List<ReactionItem>();
        public Paging Paging { get; set; } = new Paging();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}