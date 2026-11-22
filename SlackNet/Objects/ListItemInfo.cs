using System.Collections.Generic;

namespace SlackNet;

public class ListItemInfo
{
    public File List { get; set; } = null!;
    public ListItem Record { get; set; } = null!;
    public IList<ListItem>? Subtasks { get; set; }
}