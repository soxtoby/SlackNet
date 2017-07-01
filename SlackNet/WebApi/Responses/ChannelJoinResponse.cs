using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    public class ChannelJoinResponse
    {
        public Channel Channel { get; set; }
        public bool AlreadyInChannel { get; set; }
    }
}