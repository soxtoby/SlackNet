using System.Runtime.Serialization;
using SlackNet.Events;

namespace SlackNet;

/// <summary>
/// A Work Object entity for unfurling.
/// </summary>
public class WorkObjectEntity // NOCOMMIT separate types to get specific types of fields
{
    /// <summary>
    /// Entity type.
    /// </summary>
    public EntityType EntityType { get; set; }

    /// <summary>
    /// The URL link posted by the user in a conversation.
    /// This is the same URL obtained from the <see cref="LinkShared"/> event.
    /// </summary>
    public string AppUnfurlUrl { get; set; }

    /// <summary>
    /// The URL that directs the user to the desired resource on a third party system.
    /// When a user clicks the Work Object link trigger, they'll be directed to this URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Contains a string ID that uniquely identifies the resource being unfurled.
    /// It's strongly recommended to use the same ID your third-party system uses to identify and retrieve the resource via its APIs.
    /// </summary>
    public ExternalReference ExternalRef { get; set; } = new();

    /// <summary>
    /// Entity payload containing attributes, fields, and actions.
    /// </summary>
    public EntityPayload EntityPayload { get; set; } = new();
}

public enum EntityType
{
    [EnumMember(Value = "slack#/entities/file")]
    File,
    [EnumMember(Value = "slack#/entities/task")]
    Task,
    [EnumMember(Value = "slack#/entities/incident")]
    Incident,
    [EnumMember(Value = "slack#/entities/content_item")]
    ContentItem,
    [EnumMember(Value = "slack#/entities/item")]
    Item
}