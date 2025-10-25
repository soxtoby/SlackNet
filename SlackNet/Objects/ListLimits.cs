namespace SlackNet;

public class ListLimits
{
    public bool OverRowMaximum { get; set; }
    public int RowCountLimit { get; set; }
    public int RowCount { get; set; }
    public int ArchivedRowCount { get; set; }
    public bool OverColumnMaximum { get; set; }
    public int ColumnCount { get; set; }
    public int ColumnCountLimit { get; set; }
    public bool OverViewMaximum { get; set; }
    public int ViewCount { get; set; }
    public int ViewCountLimit { get; set; }
    public int MaxAttachmentsPerCell { get; set; }
}