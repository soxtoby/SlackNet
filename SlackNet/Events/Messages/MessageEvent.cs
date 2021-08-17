using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.Events
{
    [SlackType("message")]
    public class MessageEvent : Event
    {
        public string Subtype { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public string Ts { get; set; }
        public string Team { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string ThreadTs { get; set; }
        [JsonIgnore]
        public DateTime? ThreadTimestamp => ThreadTs.ToDateTime();
        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();
        public IList<Block> Blocks { get; set; } = new List<Block>();
        public Edit Edited { get; set; }
        /// <summary>
        /// Indicates message is part of the history of a channel but should not be displayed to users.
        /// </summary>
        public virtual bool Hidden => false;
        public int ReplyCount { get; set; }
        public IList<string> ReplyUsers { get; set; } = new List<string>();
        public int ReplyUsersCount { get; set; }
        public string LatestReply { get; set; }
        public bool IsStarred { get; set; }
        public IList<Reaction> Reactions { get; set; } = new List<Reaction>();
        public IList<File> Files { get; set; } = new List<File>();
        /// <summary>
        /// Indicates whether a file share happened at upload time, or some time later.
        /// </summary>
        public bool Upload { get; set; }
    }

    public class Edit
    {
        public string User { get; set; }
        public string Ts { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    }

    public class Reply
    {
        public string User { get; set; }
        public string Ts { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    }
}
