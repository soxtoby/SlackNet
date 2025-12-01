using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SlackNet.Blocks;
using ActionElement = SlackNet.Interaction.ActionElement;

namespace SlackNet;

public interface IReadOnlyAttachment
{
    IList<Block> Blocks { get; }
    IList<MessageBlock> MessageBlocks { get; }
    string Color { get; }
    string Id { get; }
    string Fallback { get; }
    string Pretext { get; }
    string AuthorName { get; }
    string AuthorSubname { get; }
    string AuthorLink { get; }
    string AuthorIcon { get; }
    string AuthorId { get; }
    string Title { get; }
    string TitleLink { get; }
    string Text { get; }
    IList<Field> Fields { get; }
    string ImageUrl { get; }
    string ThumbUrl { get; }
    int? ThumbWidth { get; }
    int? ThumbHeight { get; }
    string FromUrl { get; }
    string Footer { get; }
    string FooterIcon { get; }
    string Ts { get; }
    string CallbackId { get; }
    IList<ActionElement> Actions { get; }
    string ChannelTeam { get; }
    string ChannelId { get; }
    string ChannelName { get; }
    bool? IsShare { get; }
    bool? IsMsgUnfurl { get; }
    string AttachmentType { get; }
    string OriginalUrl { get; }
    DateTime? Timestamp { get; }
    string ServiceName { get; }
    string ServiceIcon { get; }
    bool? IsAppUnfurl { get; }
    string AppUnfurlUrl { get; }
    string BotId { get; }
    string BotTeamId { get; }
}

public class Attachment : IReadOnlyAttachment
{
    /// <summary>
    /// Structured blocks. If any blocks are specified, then no other properties can be set, except <see cref="Color"/>.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<Block> Blocks { get; set; } = [];
    [IgnoreIfEmpty]
    public IList<MessageBlock> MessageBlocks { get; set; } = [];
    public string Color { get; set; }
    public string Id { get; set; }
    public string Fallback { get; set; }
    public string Pretext { get; set; }
    public string AuthorName { get; set; }
    public string AuthorSubname { get; set; }
    public string AuthorLink { get; set; }
    public string AuthorIcon { get; set; }
    public string AuthorId { get; set; }
    public string AttachmentType { get; set; }
    public string Title { get; set; }
    public string TitleLink { get; set; }
    public string Text { get; set; }
    [IgnoreIfEmpty]
    public IList<Field> Fields { get; set; } = [];
    public string ImageUrl { get; set; }
    public string ThumbUrl { get; set; }
    public int? ThumbWidth { get; set; }
    public int? ThumbHeight { get; set; }
    public string FromUrl { get; set; }
    public string OriginalUrl { get; set; }
    public string Footer { get; set; }
    public string FooterIcon { get; set; }
    public string Ts { get; set; }
    [JsonIgnore]
    public DateTime? Timestamp => Ts?.ToDateTime().GetValueOrDefault();
    public string CallbackId { get; set; }
    [IgnoreIfEmpty]
    public IList<ActionElement> Actions { get; set; } = [];
    public string ChannelTeam { get; set; }
    public string ChannelId { get; set; }
    public string ChannelName { get; set; }
    public bool? IsShare { get; set; }
    public bool? IsMsgUnfurl { get; set; }
    public string ServiceName { get; set; }
    public string ServiceIcon { get; set; }
    public bool? IsAppUnfurl { get; set; }
    public string AppUnfurlUrl { get; set; }
    public string BotId { get; set; }
    public string BotTeamId { get; set; }
}

public class MessageBlock
{
    public string Team { get; set; }
    public string Channel { get; set; }
    public string Ts { get; set; }
    public MessageBlocks Message { get; set; }
}

public class MessageBlocks
{
    public IList<Block> Blocks { get; set; }
}