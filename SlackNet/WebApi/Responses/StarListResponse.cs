using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class StarListResponse
    {
        public IList<StarredItem> Items { get; set; } = new List<StarredItem>();
        public Paging Paging { get; set; } = new Paging();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}