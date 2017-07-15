using SlackNet.Events;

namespace SlackNet
{
    [SlackType("message")]
    public class StarredMessage : StarredItem
    {
        public MessageEvent Message { get; set; }
    }
}