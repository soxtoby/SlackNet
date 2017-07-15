namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a message in a channel is edited.
    /// </summary>
    public class MessageChanged : MessageEvent
    {
        public override bool Hidden => true;
        public MessageEvent Message { get; set; }
    }
}