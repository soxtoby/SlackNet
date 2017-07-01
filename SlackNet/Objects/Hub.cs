using System;
using SlackNet.Events.Messages;

namespace SlackNet.Objects
{
    public class Hub
    {
        public string Id { get; set; }
        /// <summary>
        /// The name of the channel, without a leading hash sign.
        /// </summary>
        public string Name { get; set; }
        public int Created { get; set; }
        public DateTime CreatedDateTime => Created.ToDateTime().GetValueOrDefault();
        /// <summary>
        /// The user ID of the member that created this channel.
        /// </summary>
        public string Creator { get; set; }
        public bool IsArchived { get; set; }
        public string[] Members { get; set; }
        public Topic Topic { get; set; }
        public Topic Purpose { get; set; }
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
    }
}