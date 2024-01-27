using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.WebApi;

public interface IReadOnlyMessage
{
    /// <summary>
    /// Channel, private group, or IM channel to send message to. Can be an encoded ID, or a name.
    /// </summary>
    string Channel { get; }

    /// <summary>
    /// Text of the message to send.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Change how messages are treated.
    /// </summary>
    ParseMode Parse { get; }

    /// <summary>
    /// Find and link channel names and usernames.
    /// </summary>
    bool LinkNames { get; }

    /// <summary>
    /// Structured message attachments.
    /// </summary>
    IList<Attachment> Attachments { get; }

    /// <summary>
    /// Structured blocks.
    /// </summary>
    IList<Block> Blocks { get; }

    /// <summary>
    /// Pass True to enable unfurling of primarily text-based content.
    /// </summary>
    bool UnfurlLinks { get; }

    /// <summary>
    /// Pass False to disable unfurling of media content.
    /// </summary>
    bool UnfurlMedia { get; }

    /// <summary>
    /// Set your bot's user name. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
    /// </summary>
    string Username { get; }

    /// <summary>
    /// Pass True to post the message as the authed user, instead of as a bot.
    /// </summary>
    bool? AsUser { get; }

    /// <summary>
    /// URL to an image to use as the icon for this message. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
    /// </summary>
    string IconUrl { get; }

    /// <summary>
    /// Emoji to use as the icon for this message. Overrides <see cref="IconUrl"/>. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
    /// </summary>
    string IconEmoji { get; }

    /// <summary>
    /// Provide another message's <see cref="MessageEventBase.Ts"/> value to make this message a reply. Avoid using a reply's ts value; use its parent instead.
    /// </summary>
    string ThreadTs { get; }

    /// <summary>
    /// Used in conjunction with <see cref="ThreadTs"/> and indicates whether reply should be made visible to everyone in the channel or conversation.
    /// </summary>
    bool ReplyBroadcast { get; }
    
    /// <summary>
    /// A custom event attached to the message. Metadata you post to Slack is accessible to any app or user who is a member of that workspace.
    /// </summary>
    /// <remarks>Will take precedence over <see cref="MetadataObject"/>.</remarks>
    MessageMetadata MetadataJson { get; }
    
    /// <summary>
    /// A custom event attached to the message. Metadata you post to Slack is accessible to any app or user who is a member of that workspace.
    /// </summary>
    /// <remarks>The specified object be automatically converted to a <see cref="MessageMetadata"/> using the standard Slack JSON conventions.</remarks>
    object MetadataObject { get; }
}

public class Message : IReadOnlyMessage
{
    public string Channel { get; set; }
    public string Text { get; set; }
    public ParseMode Parse { get; set; }
    public bool LinkNames { get; set; }
    public IList<Attachment> Attachments { get; set; } = [];
    public IList<Block> Blocks { get; set; } = [];
    public bool UnfurlLinks { get; set; }
    public bool UnfurlMedia { get; set; } = true;
    public string Username { get; set; }
    public bool? AsUser { get; set; }
    public string IconUrl { get; set; }
    public string IconEmoji { get; set; }
    public string ThreadTs { get; set; }
    public bool ReplyBroadcast { get; set; }
    public MessageMetadata MetadataJson { get; set; }
    public object MetadataObject { get; set; }
}