using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ListItemsResponse
{
    public IList<ListItem> Items { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}