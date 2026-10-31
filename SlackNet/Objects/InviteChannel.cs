namespace SlackNet;

public class InviteChannel : SlackObject
{
    public string Id { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsIm { get; set; }
    public string Name { get; set; }
}