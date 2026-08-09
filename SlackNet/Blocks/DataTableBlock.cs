using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A rich table that supports pagination, sorting, filtering, and rich interactivity.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/data-table-block/">Slack documentation</a> for more information.</remarks>
[SlackType("data_table")]
public class DataTableBlock() : Block("data_table")
{
    /// <summary>
    /// An array consisting of table rows. The first row is the header.
    /// </summary>
    public IList<IList<TableCell>> Rows { get; set; } = [];

    /// <summary>
    /// A caption for the table.
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Number of rows per page. Defaults to 5 if omitted.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// The 0-based index of the column that uniquely identifies each row for screen readers. Defaults to 0 if omitted.
    /// </summary>
    public int? RowHeaderColumnIndex { get; set; }
}
