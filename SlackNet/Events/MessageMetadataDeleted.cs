namespace SlackNet.Events;

/// <summary>
/// Sent when a message's metadata has been deleted.
/// Within the payload there will be a <see cref="PreviousMetadata"/> property notifying you of what has been deleted.
/// </summary>
public class MessageMetadataDeleted : Event
{
    public MessageMetadata PreviousMetadata { get; set; }
    public string AppId { get; set; }
    public string BotId { get; set; }
    public string UserId { get; set; }
    public string TeamId { get; set; }
    public string ChannelId { get; set; }
    public string MessageTs { get; set; }
    public string DeletedTs { get; set; }
}