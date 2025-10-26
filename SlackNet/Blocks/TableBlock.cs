using System.Collections.Generic;

namespace SlackNet.Blocks;

[SlackType("table")]
public class TableBlock() : Block("table")
{
    public IList<IList<TableCell>> Rows { get; set; } = [];
    public IList<TableColumnSettings> ColumnSettings { get; set; } = [];
}

public abstract class TableCell(string type)
{
    public string Type { get; set; } = type;
}

[SlackType("raw_text")]
public class RawTextCell() : TableCell("raw_text")
{
    public string Text { get; set; }
}

[SlackType("rich_text")]
public class RichTextCell() : TableCell("rich_text")
{
    public IList<RichTextElement> Elements { get; set; }
}

public class TableColumnSettings
{
    public ColumnAlignment Align { get; set; }
    public bool IsWrapped { get; set; }
}

public enum ColumnAlignment
{
    Left,
    Center,
    Right
}