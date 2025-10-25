using System.Collections.Generic;

namespace SlackNet;

public class ColumnSchema
{
    /// <summary>
    /// The ID of the column, provided by Slack.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// The key of the column.
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// The name of the column to be displayed in the List.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type of column.
    /// </summary>
    public ColumnType Type { get; set; }
    /// <summary>
    /// Value to specify whether the column is the primary column.
    /// Only one column in the List can be the primary column, and it must be a text column.
    /// </summary>
    public bool? IsPrimaryColumn { get; set; }
    /// <summary>
    /// Column options.
    /// </summary>
    public ColumnOptions Options { get; set; } = new();
}

public class ColumnOptions
{
    /// <summary>
    /// Used by select columns to specify options.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<ColumnOptionItem> Choices { get; set; } = new List<ColumnOptionItem>();
    /// <summary>
    /// Used by some columns such as the select column to specify some options/formatting.
    /// </summary>
    public ColumnOptionFormat? Format { get; set; }
    /// <summary>
    /// Used by numeric columns to specify the number of decimal places.
    /// </summary>
    public int? Precision { get; set; }
    /// <summary>
    /// Used by date columns to specify the format of the date.
    /// See the <a href="https://docs.slack.dev/reference/methods/slackLists.create/#schema-options-date-format">Slack documentation</a> for supported formats.
    /// </summary>
    public string DateFormat { get; set; }
    /// <summary>
    /// Used by rating and vote columns. This is the emoji to be displayed e.g., ":smile:".
    /// </summary>
    public string Emoji { get; set; }
    /// <summary>
    /// Used by rating columns, this is the team ID the emoji belongs to.
    /// </summary>
    public string EmojiTeamId { get; set; }
    /// <summary>
    /// Used by rating columns to specify the maximum rate value
    /// </summary>
    public int? Max { get; set; }
    /// <summary>
    /// Default value for some columns.
    /// </summary>
    public ColumnDefaultValue DefaultValueTyped { get; set; }
    /// <summary>
    /// Used by people, channel, and canvas columns to specify whether the entity name should be shown. Default is True.
    /// </summary>
    public bool ShowMemberName { get; set; } = true;
    /// <summary>
    /// Used by people columns to specify whether the users should be notified when the column is updated
    /// </summary>
    public bool? NotifyUsers { get; set; }
}

public class ColumnOptionItem
{
    /// <summary>
    /// The value for the option.
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// The label of the option to be displayed in the List.
    /// </summary>
    public string Label { get; set; }
    /// <summary>
    /// The color type.
    /// </summary>
    public ColumnOptionColor Color { get; set; }
}

public class ColumnDefaultValue
{
    /// <summary>
    /// Default user values (encoded user ids) for the people column.
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// Default channel values (encoded channel ids) for the channel column.
    /// </summary>
    public string Channel { get; set; }
    /// <summary>
    /// Default select values for the select column. These values should be the same ones used in the <see cref="ColumnOptions.Choices"/> value.
    /// </summary>
    public string Select { get; set; }
}

public enum ColumnType
{
    Text,
    Message,
    Number,
    Select,
    Date,
    User,
    Attachment,
    Checkbox,
    Email,
    Phone,
    Channel,
    Rating,
    CreatedBy,
    LastEditedBy,
    CreatedTime,
    LastEditedTime,
    Vote,
    Canvas,
    Reference,
    Link,
}

public enum ColumnOptionColor
{
    Indigo,
    Blue,
    Cyan,
    Pink,
    Yellow,
    Green,
    Gray,
    Red,
    Purple,
    Orange,
    Brown,
}

public enum ColumnOptionFormat
{
    /// <summary>
    /// Format used by text columns.
    /// </summary>
    Text,
    /// <summary>
    /// Format used by select columns to select a single option. 
    /// </summary>
    SingleSelect,
    /// <summary>
    /// Format used by select columns to allow multiple selections.
    /// </summary>
    MultiSelect,
    /// <summary>
    /// Format used by user and channel columns to allow only one entity in the cell.
    /// </summary>
    SingleEntity,
    /// <summary>
    /// Format used by user and channel columns to allow multiple entities in the cell.
    /// </summary>
    MultiEntity
}