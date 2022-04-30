using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.Events
{
    public abstract class MessageEventBase : Event
    {
        public Guid ClientMsgId { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public string Ts { get; set; }
        public string Team { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string Channel { get; set; }
        public string ThreadTs { get; set; }
        [JsonIgnore]
        public DateTime? ThreadTimestamp => ThreadTs.ToDateTime();
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        public IList<Block> Blocks { get; set; } = new List<Block>();
        public Edit Edited { get; set; }
        public IList<File> Files { get; set; } = new List<File>();
        /// <summary>
        /// Indicates whether a file share happened at upload time, or some time later.
        /// </summary>
        public bool Upload { get; set; }
    }
}