namespace SlackNet.Events.Messages
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
        public string EventTs { get; set; }
    }
}