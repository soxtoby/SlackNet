using System.Runtime.Serialization;
using SlackNet.Events;

namespace SlackNet;

/// <summary>
/// A Work Object entity for unfurling.
/// </summary>
public class WorkObjectEntity
{
    /// <summary>
    /// Entity type.
    /// </summary>
    public EntityType EntityType { get; set; }

    /// <summary>
    /// The URL link posted by the user in a conversation.
    /// This is the same URL obtained from the <see cref="LinkShared"/> event.
    /// </summary>
    public string? AppUnfurlUrl { get; set; }

    /// <summary>
    /// The URL that directs the user to the desired resource on a third party system.
    /// When a user clicks the Work Object link trigger, they'll be directed to this URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Contains a string ID that uniquely identifies the resource being unfurled.
    /// It's strongly recommended to use the same ID your third-party system uses to identify and retrieve the resource via its APIs.
    /// </summary>
    public ExternalReference ExternalRef { get; set; } = new();

    /// <summary>
    /// Entity payload containing attributes, fields, and actions.
    /// </summary>
    public EntityPayload EntityPayload { get; set; } = new();

    /// <summary>
    /// Adds an entry point to the comment flexpane from the unfurl card.
    /// The presence of this field indicates whether or not the Work Object entity supports comments.
    /// If your entity does not support comments, leave this null.
    /// Conversely, if your entity has 0 comments, setting <see cref="EntityCommentSummary.Count"/> to 0 renders an unfurl card that links to the comment flexpane.
    /// Comments are requested with the <see cref="EntityCommentsRequested"/> event.
    /// </summary>
    public EntityCommentSummary? EntityComments { get; set; }
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