using System;
using SlackNet.Events.Messages;

namespace SlackNet.Objects
{
    public class Hub
    {
        public string Id { get; set; }
        public bool IsIm { get; set; }
        public bool IsChannel { get; set; }
        public bool IsGroup { get; set; }
        public bool IsMpim { get; set; }
        public bool IsOrgShared { get; set; }
        public int Created { get; set; }
        public DateTime CreatedDateTime => Created.ToDateTime().GetValueOrDefault();
        public string[] Members { get; set; }

        /// <summary>
        /// Timestamp for the last message the calling user has read in this channel.
        /// </summary>
        public string LastRead { get; set; }
        /// <summary>
        /// The latest message in the channel.
        /// </summary>
        public Message Latest { get; set; }
        /// <summary>
        /// Full count of visible messages that the calling user has yet to read.
        /// </summary>
        public int UnreadCount { get; set; }
        /// <summary>
        /// Count of messages that the calling user has yet to read that matter to them (this means it excludes things like join/leave messages).
        /// </summary>
        public int UnreadCountDisplay { get; set; }
        /// <summary>
        /// True if this channel is the "general" channel that includes all regular team members.
        /// </summary>
        public bool IsGeneral { get; set; }
        /// <summary>
        /// True if the calling member is part of the channel.
        /// </summary>
        public bool IsMember { get; set; }
    }
}