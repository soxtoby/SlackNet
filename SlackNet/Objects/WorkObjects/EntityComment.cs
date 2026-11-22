using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet;

/// <summary>
/// A comment on a Work Object, presented in the comments tab of its flexpane.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/messaging/work-objects-comments/">Slack documentation</a> for more information.</remarks>
public class EntityComment
{
    /// <summary>
    /// A unique ID sourced from your app and associated with the comment. It will be used if editing a comment.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Same <see cref="EntityUser"/> type used in a Work Object's fields.
    /// </summary>
    public EntityUser Sender { get; set; } = new();

    /// <summary>
    /// Epoch timestamp in seconds.
    /// </summary>
    public long Timestamp { get; set; }

    [JsonIgnore]
    public DateTime? DateTime
    {
        get => Timestamp.ToDateTime();
        set => Timestamp = value?.ToTimestampNumber() ?? 0;
    }

    /// <summary>
    /// The comment content.
    /// </summary>
    public EntityCommentContent Comment { get; set; } = new();

    /// <summary>
    /// URL which directs to the comment.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The <see cref="Id"/> of the parent comment. If present, this comment is a threaded reply.
    /// </summary>
    public string? ThreadRootId { get; set; }

    /// <summary>
    /// If true, indicates that the comment is editable in the user's flexpane.
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// If true, renders the delete button on that comment.
    /// When true, the <c>deleteActionId</c> passed to <see cref="WebApi.IEntityApi.PresentComments"/> should also be present,
    /// otherwise the delete button will render but do nothing when clicked.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// If true, allows users to reply to this comment in a thread.
    /// </summary>
    public bool CanReply { get; set; }

    /// <summary>
    /// Number of replies in the thread under this comment.
    /// </summary>
    public int? ReplyCount { get; set; }

    /// <summary>
    /// Users who have replied to this comment thread.
    /// </summary>
    public IList<EntityUser> ReplyUsers { get; set; } = [];

    /// <summary>
    /// Epoch timestamp in seconds of the most recent reply in the thread.
    /// </summary>
    public long? LatestReplyTs { get; set; }

    [JsonIgnore]
    public DateTime? LatestReplyTimestamp
    {
        get => LatestReplyTs?.ToDateTime();
        set => LatestReplyTs = value?.ToTimestampNumber();
    }

    /// <summary>
    /// Epoch timestamp in seconds of the last edit to this comment.
    /// </summary>
    public long? LastEditTs { get; set; }

    [JsonIgnore]
    public DateTime? LastEditTimestamp
    {
        get => LastEditTs?.ToDateTime();
        set => LastEditTs = value?.ToTimestampNumber();
    }

    /// <summary>
    /// Pagination cursor associated with this comment's position in the list.
    /// </summary>
    public string? Cursor { get; set; }
}

/// <summary>
/// The content of an <see cref="EntityComment"/>. Provide one of <see cref="Blocks"/> or <see cref="Value"/>.
/// </summary>
public class EntityCommentContent
{
    /// <summary>
    /// A rich text block containing the comment content.
    /// </summary>
    public RichTextBlock? Blocks { get; set; }

    /// <summary>
    /// Plain or markdown string containing the comment content. This will be used if <see cref="Blocks"/> is not present on the comment object.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// If set to "markdown", the <see cref="Value"/> will be formatted as markdown.
    /// </summary>
    public string? Format { get; set; }
}

/// <summary>
/// A summary of the comments on a <see cref="WorkObjectEntity"/>.
/// </summary>
public class EntityCommentSummary
{
    /// <summary>
    /// The number of comments on the entity.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Epoch timestamp in seconds of the most recent comment.
    /// </summary>
    public long? LatestTimestamp { get; set; }

    [JsonIgnore]
    public DateTime? LatestDateTime
    {
        get => LatestTimestamp?.ToDateTime();
        set => LatestTimestamp = value?.ToTimestampNumber();
    }
}
