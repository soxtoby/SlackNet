using System;

namespace SlackNet
{
    public class Link
    {
        public string Id { get; }
        public string Caption { get; }

        public Link(string id, string caption)
        {
            Id = id;
            Caption = caption;
        }

        public override string ToString() => string.IsNullOrEmpty(Caption)
            ? $"<{Id}>"
            : $"<{Id}|{Caption}>";

        public static Link Url(string url, string caption = null) => new Link(url, caption);
        public static Link User(string userId) => new Link('@' + userId, null);
        [Obsolete("Use Conversation instead")]
        public static Link Hub(string hubId) => new Link('#' + hubId, null);
        public static Link Conversation(string conversationId) => new Link('#' + conversationId, null);
    }
}