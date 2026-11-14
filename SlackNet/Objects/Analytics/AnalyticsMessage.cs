#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Structural information about a message, without the actual message content.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.messages.metadata/#message-object-fields">Slack documentation</a> for more information.</remarks>
public class AnalyticsMessage
{
    /// <summary>
    /// Message type.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Timestamp of the message.
    /// </summary>
    public required string Ts { get; set; }

    /// <summary>
    /// Encoded ID of the user who authored the message.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Message subtype (e.g., <c>bot_message</c>, <c>channel_join</c>).
    /// </summary>
    public string? Subtype { get; set; }

    /// <summary>
    /// Timestamp of the root message if this is a threaded reply.
    /// </summary>
    public string? ThreadTs { get; set; }

    /// <summary>
    /// Whether the message displays as a bot.
    /// </summary>
    public bool DisplayAsBot { get; set; }

    /// <summary>
    /// Client-generated message ID.
    /// </summary>
    public string? ClientMsgId { get; set; }

    /// <summary>
    /// Number of characters in the message text.
    /// </summary>
    public int TextCharacterCount { get; set; }

    /// <summary>
    /// Reaction counts. <see cref="Reaction.Users"/> is not included.
    /// </summary>
    public IList<Reaction> Reactions { get; set; } = [];

    /// <summary>
    /// Metadata of files attached to the message.
    /// </summary>
    public IList<File> Files { get; set; } = [];
}
