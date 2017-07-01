namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when the purpose for a channel is changed.
    /// </summary>
    public class ChannelPurpose : Message
    {
        public string Purpose { get; set; }
    }
}