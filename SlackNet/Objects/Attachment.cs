using System;
using System.Collections.Generic;

namespace SlackNet
{
    public class Attachment
    {
        public string Fallback { get; set; }
        public string Color { get; set; }
        public string Pretext { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLink { get; set; }
        public string AuthorIcon { get; set; }
        public string Title { get; set; }
        public string TitleLink { get; set; }
        public string Text { get; set; }
        public IList<Field> Fields { get; } = new List<Field>();
        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string Footer { get; set; }
        public string FooterIcon { get; set; }
        public int Ts { get; set; }
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public IList<Action> Action { get; } = new List<Action>();
    }
}