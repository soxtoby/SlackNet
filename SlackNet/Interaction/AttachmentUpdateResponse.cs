using System;
using SlackNet.Blocks;
using System.Collections.Generic;
using System.Linq;

namespace SlackNet.Interaction;

public class AttachmentUpdateResponse : IReadOnlyAttachment
{
    private readonly MessageResponse _response;

    public AttachmentUpdateResponse(MessageResponse response) => _response = response;

    public ResponseType ResponseType => _response.ResponseType;
    public bool ReplaceOriginal => _response.ReplaceOriginal;
    public bool DeleteOriginal => _response.DeleteOriginal;
    public IList<MessageBlock> MessageBlocks => Attachment.MessageBlocks;
    public IList<Block> Blocks => Attachment.Blocks;
    public string Color => Attachment.Color;
    public string Id => Attachment.Id;
    public string Fallback => Attachment.Fallback;
    public string Pretext => Attachment.Pretext;
    public string AuthorId => Attachment.AuthorId;
    public string AuthorName => Attachment.AuthorName;
    public string AuthorSubname => Attachment.AuthorSubname;
    public string AuthorLink => Attachment.AuthorLink;
    public string AuthorIcon => Attachment.AuthorIcon;
    public string Title => Attachment.Title;
    public string TitleLink => Attachment.TitleLink;
    public string Text => Attachment.Text;
    public IList<Field> Fields => Attachment.Fields;
    public string ImageUrl => Attachment.ImageUrl;
    public string ThumbUrl => Attachment.ThumbUrl;
    public int? ThumbWidth => Attachment.ThumbWidth;
    public int? ThumbHeight => Attachment.ThumbHeight;
    public string FromUrl => Attachment.FromUrl;
    public string Footer => Attachment.Footer;
    public string FooterIcon => Attachment.FooterIcon;
    public string Ts => Attachment.Ts;
    public string CallbackId => Attachment.CallbackId;
    public IList<ActionElement> Actions => Attachment.Actions;
    public string ChannelTeam => Attachment.ChannelTeam;
    public string ChannelId => Attachment.ChannelId;
    public string ChannelName => Attachment.ChannelName;
    public bool? IsShare => Attachment.IsShare;
    public bool? IsMsgUnfurl => Attachment.IsMsgUnfurl;
    public string AttachmentType => Attachment.AttachmentType;
    public string OriginalUrl => Attachment.OriginalUrl;
    public DateTime? Timestamp => Attachment.Timestamp;
    public string ServiceName => Attachment.ServiceName;
    public string ServiceIcon => Attachment.ServiceIcon;
    public bool? IsAppUnfurl => Attachment.IsAppUnfurl;
    public string AppUnfurlUrl => Attachment.AppUnfurlUrl;
    public string BotId => Attachment.BotId;
    public string BotTeamId => Attachment.BotTeamId;

    private Attachment Attachment => _response.Message.Attachments.First();
}
