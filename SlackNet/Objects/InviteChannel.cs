namespace SlackNet;

public class InviteChannel
{
    public string Id { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsIm { get; set; }
    public string Name { get; set; }
    public int DateCreated { get; set; }
    public long MessageLatestCountedTimestamp { get; set; }
}