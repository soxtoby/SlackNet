using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ListItemsResponse : SlackObject
{
    public IList<ListItem> Items { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}