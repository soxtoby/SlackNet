using System;
using System.Collections.Generic;
using System.Linq;
using SlackNet.Blocks;

namespace SlackNet.Interaction
{
    public class AttachmentUpdateResponse: IReadOnlyAttachment
    {
        private readonly MessageResponse _response;

        public AttachmentUpdateResponse(MessageResponse response) => _response = response;

        public ResponseType ResponseType => _response.ResponseType;
        public bool ReplaceOriginal => _response.ReplaceOriginal;
        public bool DeleteOriginal => _response.DeleteOriginal;
        public IList<Block> Blocks => Attachment.Blocks;
        public string Color => Attachment.Color;
        public string Id => Attachment.Id;
        public string Fallback => Attachment.Fallback;
        public string Pretext => Attachment.Pretext;
        public string AuthorName => Attachment.AuthorName;
        public string AuthorLink => Attachment.AuthorLink;
        public string AuthorIcon => Attachment.AuthorIcon;
        public string Title => Attachment.Title;
        public string TitleLink => Attachment.TitleLink;
        public string Text => Attachment.Text;
        public IList<Field> Fields => Attachment.Fields;
        public string ImageUrl => Attachment.ImageUrl;
        public string ThumbUrl => Attachment.ThumbUrl;
        public string Footer => Attachment.Footer;
        public string FooterIcon => Attachment.FooterIcon;
        public int? Ts => Attachment.Ts;
        public string CallbackId => Attachment.CallbackId;
        public IList<ActionElement> Actions => Attachment.Actions;

        private Attachment Attachment => _response.Message.Attachments.First();
    }
}