using System.Collections.Generic;
using SlackNet.WebApi;

namespace SlackNet.Bot
{
    public class BotMessage
    {
        /// <summary>
        /// Channel, private group, or IM channel to send message to. Can be an encoded ID, or a name.
        /// </summary>
        public HubIdentifier Hub { get; set; }
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
        /// Message being replied to. Reply will be in same channel & thread by default.
        /// </summary>
        public SlackMessage ReplyTo { get; set; }
        /// <summary>
        /// Indicates whether message in a thread should be made visible to everyone in the channel or conversation. 
        /// </summary>
        public bool ReplyBroadcast { get; set; }
        /// <summary>
        /// Set to True to reply in a new thread if not already in one.
        /// </summary>
        public bool CreateThread { get; set; }
    }
}
