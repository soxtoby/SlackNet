using System.Collections.Generic;

namespace SlackNet;

public class ListView
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ListViewType Type { get; set; }
    public bool IsLocked { get; set; }
    public string Position { get; set; }
    public IList<ColumnView> Columns { get; set; }
    public int DateCreated { get; set; }
    public string CreatedBy { get; set; }
    public bool StickColumnLeft { get; set; }
    public bool IsAllItemsView { get; set; }
    public string DefaultViewKey { get; set; }
    public bool ShowCompletedItems { get; set; }
}

public class ColumnView
{
    public bool Visible { get; set; }
    public string Key { get; set; }
    public string Id { get; set; }
    public string Position { get; set; }
    public int? Width { get; set; }
    public bool? ShouldWrapText { get; set; }
}

public enum ListViewType
{
    Record,
    Table
}