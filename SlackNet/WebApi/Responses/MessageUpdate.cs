using System.Collections.Generic;
using SlackNet.Blocks;

namespace SlackNet.WebApi;

public class MessageUpdate
{
    /// <summary>
    /// Timestamp of the message to be updated.
    /// </summary>
    public string Ts { get; set; }
    /// <summary>
    /// Channel containing the message to be updated.
    /// </summary>
    public string ChannelId { get; set; }
    /// <summary>
    /// New text for the message, using the default formatting rules.
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// Structured message attachments. Leave as null to leave existing attachments alone.
    /// </summary>
    public IList<Attachment> Attachments { get; set; }
    /// <summary>
    /// Structured blocks. Leave as null to leave existing blocks alone.
    /// </summary>
    public IList<Block> Blocks { get; set; }
    /// <summary>
    /// Change how messages are treated.
    /// </summary>
    public ParseMode Parse { get; set; } = ParseMode.Client;
    /// <summary>
    /// Find and link channel names and usernames.
    /// This property should be used in conjunction with <see cref="Parse"/>. To set <see cref="LinkNames"/> to True, specify a parse mode of <see cref="ParseMode.Full"/>.
    /// </summary>
    public bool LinkNames { get; set; }
    /// <summary>
    /// Set to True to update the message as the authed user. Bot users in this context are considered authed users.
    /// </summary>
    public bool? AsUser { get; set; }
    /// <summary>
    /// Array of new file ids that will be sent with this message.
    /// </summary>
    public IList<string> FileIds { get; set; }
}