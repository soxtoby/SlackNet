using System;
using System.Collections.Generic;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNet.WebApi
{
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
        [Obsolete("as_user: This argument may not be used with newer bot tokens.")]
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
        /// Provide another message's <see cref="MessageEvent.Ts"/> value to make this message a reply. Avoid using a reply's ts value; use its parent instead.
        /// </summary>
        string ThreadTs { get; }

        /// <summary>
        /// Used in conjunction with <see cref="ThreadTs"/> and indicates whether reply should be made visible to everyone in the channel or conversation. 
        /// </summary>
        bool ReplyBroadcast { get; }
    }

    public class Message : IReadOnlyMessage
    {
        /// <summary>
        /// Channel, private group, or IM channel to send message to. Can be an encoded ID, or a name.
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// Text of the message to send.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Change how messages are treated.
        /// </summary>
        public ParseMode Parse { get; set; }
        /// <summary>
        /// Find and link channel names and usernames.
        /// </summary>
        public bool LinkNames { get; set; }
        /// <summary>
        /// Structured message attachments.
        /// </summary>
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        /// <summary>
        /// Structured blocks.
        /// </summary>
        public IList<Block> Blocks { get; set; } = new List<Block>();
        /// <summary>
        /// Pass True to enable unfurling of primarily text-based content.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public bool UnfurlLinks { get; set; }
        /// <summary>
        /// Pass False to disable unfurling of media content.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public bool UnfurlMedia { get; set; } = true;
        /// <summary>
        /// Set your bot's user name. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Pass True to post the message as the authed user, instead of as a bot.
        /// </summary>
        [Obsolete("as_user: This argument may not be used with newer bot tokens.")]
        public bool? AsUser { get; set; }
        /// <summary>
        /// URL to an image to use as the icon for this message. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// Emoji to use as the icon for this message. Overrides <see cref="IconUrl"/>. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public string IconEmoji { get; set; }
        /// <summary>
        /// Provide another message's <see cref="MessageEvent.Ts"/> value to make this message a reply. Avoid using a reply's <c>Ts</c> value; use its parent instead.
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public string ThreadTs { get; set; }
        /// <summary>
        /// Used in conjunction with <see cref="ThreadTs"/> and indicates whether reply should be made visible to everyone in the channel or conversation. 
        /// Not supported when posting an ephemeral message.
        /// </summary>
        public bool ReplyBroadcast { get; set; }
    }
}