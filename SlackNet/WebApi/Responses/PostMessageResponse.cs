using SlackNet.Events.Messages;

namespace SlackNet.WebApi.Responses
{
    public class PostMessageResponse
    {
        public string Ts { get; set; }
        public string Channel { get; set; }
        public Message Message { get; set; }
    }
}