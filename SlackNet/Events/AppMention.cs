using Newtonsoft.Json;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// Subscribe to only the message events that mention your app or bot.
    /// </summary>
    public class AppMention : Event
    {
        public string User { get; set; }
        public string Text { get; set; }
        public string Ts { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string Channel { get; set; }
        public string EventTs { get; set; }
        public string Subtype { get; set; }
        public string ThreadTs { get; set; }
        [JsonIgnore]
        public DateTime? ThreadTimestamp => ThreadTs.ToDateTime();
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        public IList<Block> Blocks { get; set; } = new List<Block>();
        public Edit Edited { get; set; }
        public IList<File> Files { get; set; } = new List<File>();
        public bool Upload { get; set; }
    }
}