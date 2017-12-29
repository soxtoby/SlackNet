using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet
{
    public interface IReadOnlyAttachment
    {
        string Id { get; }
        string Fallback { get; }
        string Color { get; }
        string Pretext { get; }
        string AuthorName { get; }
        string AuthorLink { get; }
        string AuthorIcon { get; }
        string Title { get; }
        string TitleLink { get; }
        string Text { get; }
        IList<Field> Fields { get; }
        string ImageUrl { get; }
        string ThumbUrl { get; }
        string Footer { get; }
        string FooterIcon { get; }
        int Ts { get; }
        DateTime Timestamp { get; }
        string CallbackId { get; }
        IList<Action> Actions { get; }
    }

    public class Attachment : IReadOnlyAttachment
    {
        public string Id { get; set; }
        public string Fallback { get; set; }
        public string Color { get; set; }
        public string Pretext { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLink { get; set; }
        public string AuthorIcon { get; set; }
        public string AttachmentType { get; set; }
        public string Title { get; set; }
        public string TitleLink { get; set; }
        public string Text { get; set; }
        public IList<Field> Fields { get; set; } = new List<Field>();
        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string Footer { get; set; }
        public string FooterIcon { get; set; }
        public int Ts { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string CallbackId { get; set; }
        public IList<Action> Actions { get; set; } = new List<Action>();
    }
}