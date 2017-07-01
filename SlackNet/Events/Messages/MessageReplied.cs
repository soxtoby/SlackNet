namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a channel's message is a reply to another message.
    /// </summary>
    public class MessageReplied : Message
    {
        public override bool Hidden => true;
        /// <summary>
        /// The message being replied to.
        /// </summary>
        public Message Message { get; set; }
        public string EventTs { get; set; }
    }
}