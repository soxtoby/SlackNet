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
        public static Link Hub(string hubId) => new Link('#' + hubId, null);
    }
}