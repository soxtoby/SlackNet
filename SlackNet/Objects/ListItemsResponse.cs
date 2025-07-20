using System.Collections.Generic;
using SlackNet.WebApi;

namespace SlackNet;

public class ListItemsResponse
{
    public IList<ListItem> Items { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}