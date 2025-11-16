using System.Collections.Generic;

namespace SlackNet;

public class EntityPayload
{
    public EntityAttributes Attributes { get; set; } = new();
    public StandardEntityFields Fields { get; set; } = new();
    [IgnoreIfEmpty]
    public IList<EntityField> CustomFields { get; set; } = [];
    [IgnoreIfEmpty]
    public IList<string> DisplayOrder { get; set; } = [];
    public EntityActions Actions { get; set; }
    public EntityRemoteFile SlackFile { get; set; }
}

public class StandardEntityFields
{
    public ImageField Preview { get; set; }
    public UserField CreatedBy { get; set; }
    public TimestampField DateCreated { get; set; }
    public TimestampField DateUpdated { get; set; }
    public UserField LastModifiedBy { get; set; }
    public UserField Assignee { get; set; }
    public StringField FileSize { get; set; }
    public StringField MimeType { get; set; }
    public StringField Description { get; set; }
    public DateField DueDate { get; set; }
    public StringField Status { get; set; }
    public StringField Priority { get; set; }
    public StringField Severity { get; set; }
    public StringField Service { get; set; }
}

public class EntityActions
{
    public IList<EntityActionButton> PrimaryActions { get; set; } = [];
    public IList<EntityActionButton> OverflowActions { get; set; } = [];
}

public class EntityRemoteFile
{
    /// <summary>
    /// Apps providing remote files as well as Work Objects should provide the 
    /// file ID to ensure compatibility with Slack's unified files browser.
    /// </summary>
    public string Id { get; set; }
    
    /// <summary>
    /// The file's extension can optionally be specified here to create an icon for the file using built-in file icons.
    /// </summary>
    public string Type { get; set; }
}