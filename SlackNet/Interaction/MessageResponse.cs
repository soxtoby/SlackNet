using SlackNet.WebApi;

namespace SlackNet.Interaction
{
    public class MessageResponse
    {
        public ResponseType ResponseType { get; set; }
        public bool ReplaceOriginal { get; set; }
        public bool DeleteOriginal { get; set; }
        public Message Message { get; set; }
    }
}