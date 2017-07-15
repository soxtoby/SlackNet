using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class StarListResponse
    {
        public IList<StarredItem> Items { get; set; }
        public Paging Paging { get; set; }
    }
}