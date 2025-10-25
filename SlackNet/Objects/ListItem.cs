using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet;

public class ListItem
{
    public string Id { get; set; }
    public string ListId { get; set; }
    public int DateCreated { get; set; }
    public DateTime? Created => DateCreated.ToDateTime();
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public bool Archived { get; set; }
    public string UpdatedTimestamp { get; set; }
    public DateTime? Updated => UpdatedTimestamp.ToDateTime();
    public string ParentRecordId { get; set; }
    public IList<ListItemField> Fields { get; set; } = [];
    public bool? IsSubscribed { get; set; }
}

public class ListItemField
{
    public string Key { get; set; }
    public string ColumnId { get; set; }
    public string Value { get; set; }
    public string Text { get; set; }
    public IList<RichTextBlock> RichText { get; set; }
    public IList<string> Date { get; set; }
    public IList<string> Select { get; set; }
    public IList<string> User { get; set; }
    public bool? Checkbox { get; set; }
    public IList<string> Email { get; set; }
    public IList<string> Phone { get; set; }
    public IList<string> Channel { get; set; }
    public IList<int> Rating { get; set; }
    public IList<int> Timestamp { get; set; }
    public IList<ListItemLink> Link { get; set; }
    public IList<string> Attachment { get; set; }
    public IList<double> Number { get; set; }
    public IList<MessageReference> Message { get; set; }
    public IList<ListItemReference> Reference { get; set; }
}

public class ListItemFieldUpdate : ListItemField
{
    public string RowId { get; set; }
}

public class MessageReference
{
    public string Value { get; set; }
    public string ChannelId { get; set; }
    public string Ts { get; set; }
    public string ThreadTs { get; set; }
}

public class ListItemLink
{
    [JsonProperty("originalUrl")]
    public string OriginalUrl { get; set; }
    public IDictionary<string, Attachment> Attachment { get; set; }
    [JsonProperty("displayAsUrl")]
    public bool DisplayAsUrl { get; set; }
}

public class ListItemReference
{
    public FileReference File { get; set; }
}

public class FileReference
{
    public string FileId { get; set; }
}