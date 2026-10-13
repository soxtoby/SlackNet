using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;

namespace SlackNet.Events;

public abstract class MessageEventBase : Event
{
    public Guid ClientMsgId { get; set; }
    public string User { get; set; }
    public string Text { get; set; }
    public string Ts { get; set; }
    public string Team { get; set; }
    /// <summary>
    /// The workspace of the user who sent the message. Only sent when the message comes from a channel that is shared between workspaces.
    /// </summary>
    public string UserTeam { get; set; }
    /// <summary>
    /// The workspace the message was sent from. Only sent when the message comes from a channel that is shared between workspaces.
    /// </summary>
    public string SourceTeam { get; set; }
    public bool DisplayAsBot { get; set; }
    [JsonIgnore]
    public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
    public string Channel { get; set; }
    public string ThreadTs { get; set; }
    [JsonIgnore]
    public DateTime? ThreadTimestamp => ThreadTs.ToDateTime();
    public IList<Attachment> Attachments { get; set; } = [];
    public IList<Block> Blocks { get; set; } = [];
    public Edit Edited { get; set; }
    public IList<File> Files { get; set; } = [];
    /// <summary>
    /// Indicates whether a file share happened at upload time, or some time later.
    /// </summary>
    public bool Upload { get; set; }
}