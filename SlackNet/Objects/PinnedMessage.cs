using SlackNet.Events.Messages;

namespace SlackNet.Objects
{
    [SlackType("message")]
    public class PinnedMessage : PinnedItem
    {
        /// <summary>
        /// The channel ID for the message.
        /// </summary>
        public string Channel { get; set; }
        public MessageEvent Message { get; set; }
    }
}