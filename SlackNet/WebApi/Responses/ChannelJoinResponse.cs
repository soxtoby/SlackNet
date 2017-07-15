namespace SlackNet.WebApi
{
    public class ChannelJoinResponse
    {
        public Channel Channel { get; set; }
        public bool AlreadyInChannel { get; set; }
    }
}