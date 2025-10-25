using System.Collections.Generic;

namespace SlackNet;

public class ListItemInfo
{
    public File List { get; set; }
    public ListItem Record { get; set; }
    public IList<ListItem> Subtasks { get; set; }
}