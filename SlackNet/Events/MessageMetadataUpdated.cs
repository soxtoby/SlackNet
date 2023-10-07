namespace SlackNet.Events;

/// <summary>
/// Sent when a message's metadata has been updated.
/// Within the payload you'll notice an extra <see cref="PreviousMetadata"/> in addition to the <see cref="Metadata"/> property, notifying you of what was changed.
/// </summary>
public class MessageMetadataUpdated : Event
{
    public MessageMetadata PreviousMetadata { get; set; }
    public MessageMetadata Metadata { get; set; }
    public string AppId { get; set; }
    public string BotId { get; set; }
    public string UserId { get; set; }
    public string TeamId { get; set; }
    public string ChannelId { get; set; }
    public string MessageTs { get; set; }
}