using SlackNet.Events.Messages;

namespace SlackNet.Objects
{
    [SlackType("message")]
    public class StarredMessage : StarredItem
    {
        public Message Message { get; set; }
    }
}