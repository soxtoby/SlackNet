using System.Collections.Generic;
using SlackNet.Events.Messages;
using SlackNet.Objects;

namespace SlackNet.WebApi
{
    public class Message
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
        public IList<Attachment> Attachments { get; } = new List<Attachment>();
        /// <summary>
        /// Pass True to enable unfurling of primarily text-based content.
        /// </summary>
        public bool UnfurlLinks { get; set; }
        /// <summary>
        /// Pass False to disable unfurling of media content.
        /// </summary>
        public bool UnfurlMedia { get; set; } = true;
        /// <summary>
        /// Set your bot's user name. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Pass True to post the message as the authed user, instead of as a bot.
        /// </summary>
        public bool AsUser { get; set; }
        /// <summary>
        /// URL to an image to use as the icon for this message. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// Emoji to use as the icon for this message. Overrides <see cref="IconUrl"/>. Must be used in conjunction with <see cref="AsUser"/> set to False, otherwise ignored.
        /// </summary>
        public string IconEmoji { get; set; }
        /// <summary>
        /// Provide another message's <see cref="MessageEvent.Ts"/> value to make this message a reply. Avoid using a reply's ts value; use its parent instead.
        /// </summary>
        public string ThreadTs { get; set; }
        /// <summary>
        /// Used in conjunction with <see cref="ThreadTs"/> and indicates whether reply should be made visible to everyone in the channel or conversation. 
        /// </summary>
        public bool ReplyBroadcast { get; set; }
    }
}