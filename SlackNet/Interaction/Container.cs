namespace SlackNet.Interaction;

public class Container
{
    public string Type { get; set; }
    public string MessageTs { get; set; }
    public int AttachmentId { get; set; }
    public string ChannelId { get; set; }
    public bool IsEphemeral { get; set; }
    public bool IsAppUnfurl { get; set; }
    public string EntityUrl { get; set; }
    public ExternalReference ExternalRef { get; set; }
    public string AppUnfurlUrl { get; set; }
    public string ThreadTs { get; set; }
}