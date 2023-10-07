using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Events;

[SlackType("message")]
public class MessageEvent : MessageEventBase
{
    public string Subtype { get; set; }
    public string ChannelType { get; set; }
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
    public IList<string> PinnedTo { get; set; } = new List<string>();
    public PinnedInfo PinnedInfo { get; set; }
    public MessageMetadata Metadata { get; set; }
    public string BotId { get; set; }
    public string AppId { get; set; }
    public BotInfo BotProfile { get; set; }
}

public class PinnedInfo
{
    public string Channel { get; set; }
    public string PinnedBy { get; set; }
    public int? PinnedTs { get; set; }
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