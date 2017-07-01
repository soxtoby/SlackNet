using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class PinnedItemListResponse
    {
        public List<PinnedItem> Items { get; set; }
    }
}