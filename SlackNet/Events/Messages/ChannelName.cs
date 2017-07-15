namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a channel is renamed.
    /// </summary>
    public class ChannelName : MessageEvent
    {
        public string OldName { get; set; }
        public string Name { get; set; }
    }
}