namespace SlackNet.Events
{
    public class ChannelIdChanged : Event
    {
        public string OldChannelId { get; set; }
        public string NewChannelId { get; set; }
    }
}