namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when the topic for a channel is changed.
    /// </summary>
    public class ChannelTopic : Message
    {
        public string Topic { get; set; }
    }
}