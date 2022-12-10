namespace SlackNet;

public class Bookmark
{
    public string Id { get; set; }
    public string ChannelId { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public string Emoji { get; set; }
    public string IconUrl { get; set; }
    public BookmarkType Type { get; set; }
    public string EntityId { get; set; }
    public int DateCreated { get; set; }
    public int DateUpdated { get; set; }
    public string Rank { get; set; }
    public string LastUpdatedByUserId { get; set; }
    public string LastUpdatedByTeamId { get; set; }
    public string ShortcutId { get; set; }
    public string AppId { get; set; }
}