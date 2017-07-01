using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class StarListResponse
    {
        public IList<StarredItem> Items { get; set; }
        public Paging Paging { get; set; }
    }
}