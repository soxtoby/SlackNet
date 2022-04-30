namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a channel's message is a reply to another message.
    /// </summary>
    public class MessageReplied : MessageEvent
    {
        public override bool Hidden => true;
        /// <summary>
        /// The message being replied to.
        /// </summary>
        public MessageEvent Message { get; set; }
    }
}