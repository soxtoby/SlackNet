using System;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a message in a channel is deleted.
    /// </summary>
    public class MessageDeleted : Message
    {
        public override bool Hidden => true;
        /// <summary>
        /// The timestamp of the message that was deleted.
        /// </summary>
        public string DeletedTs { get; set; }
        public DateTime DeleteTimestamp => DeletedTs.ToDateTime().GetValueOrDefault();
    }
}