namespace SlackNet.Events;

/// <summary>
/// Sent when a message containing metadata is sent in Slack.
/// </summary>
public class MessageMetadataPosted : Event
{
    public MessageMetadata Metadata { get; set; }
    public string AppId { get; set; }
    public string BotId { get; set; }
    public string UserId { get; set; }
    public string TeamId { get; set; }
    public string ChannelId { get; set; }
    public string MessageTs { get; set; }
}