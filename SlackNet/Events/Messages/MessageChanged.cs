namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a message in a channel is edited.
    /// </summary>
    public class MessageChanged : Message
    {
        public override bool Hidden => true;
        public Message Message { get; set; }
    }
}