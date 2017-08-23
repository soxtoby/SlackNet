using System;
using System.Collections.Generic;

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
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string ThreadTs { get; set; }
        public DateTime? ThreadTimestamp => ThreadTs.ToDateTime();
        public IList<Attachment> Attachments { get; } = new List<Attachment>();
        public Edit Edited { get; set; }
        /// <summary>
        /// Indicates message is part of the history of a channel but should not be displayed to users.
        /// </summary>
        public virtual bool Hidden => false;
        public int ReplyCount { get; set; }
        public IList<Reply> Replies { get; set; } = new List<Reply>();
        public bool IsStarred { get; set; }
        public IList<Reaction> Reactions { get; set; } = new List<Reaction>();
    }

    public class Edit
    {
        public string User { get; set; }
        public string Ts { get; set; }
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    }

    public class Reply
    {
        public string User { get; set; }
        public string Ts { get; set; }
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    }
}
