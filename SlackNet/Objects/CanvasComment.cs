using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// A comment thread on a canvas.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.conversations.getCanvas/">Slack documentation</a> for more information.</remarks>
public class CanvasComment
{
    /// <summary>
    /// Stable ID of the comment thread (the canvas annotation the comment is anchored to).
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Message timestamp of the comment.
    /// </summary>
    public required string Ts { get; set; }

    /// <summary>
    /// Encoded ID of the user who wrote the comment.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Text of the comment.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// The highlighted canvas text the comment is anchored to, rendered as mrkdwn.
    /// Empty if the anchored text has since been deleted from the canvas.
    /// </summary>
    public required string QuotedText { get; set; }

    /// <summary>
    /// Whether the comment thread has been resolved.
    /// </summary>
    public bool IsResolved { get; set; }

    /// <summary>
    /// Threaded replies to the comment, ordered oldest first.
    /// </summary>
    public IList<CanvasCommentReply> Replies { get; set; } = [];

    /// <summary>
    /// True if the thread had more replies than could be returned, and older replies were dropped from <see cref="Replies"/>.
    /// </summary>
    public bool HasMoreReplies { get; set; }
}

/// <summary>
/// A threaded reply to a <see cref="CanvasComment"/>.
/// </summary>
public class CanvasCommentReply
{
    /// <summary>
    /// Message timestamp of the reply.
    /// </summary>
    public required string Ts { get; set; }

    /// <summary>
    /// Encoded ID of the user who wrote the reply.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Text of the reply.
    /// </summary>
    public required string Text { get; set; }
}
