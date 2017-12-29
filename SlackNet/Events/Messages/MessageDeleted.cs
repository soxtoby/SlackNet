using System;
using Newtonsoft.Json;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a message in a channel is deleted.
    /// </summary>
    public class MessageDeleted : MessageEvent
    {
        public override bool Hidden => true;
        /// <summary>
        /// The timestamp of the message that was deleted.
        /// </summary>
        public string DeletedTs { get; set; }
        [JsonIgnore]
        public DateTime DeleteTimestamp => DeletedTs.ToDateTime().GetValueOrDefault();
    }
}