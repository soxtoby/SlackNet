using SlackNet.WebApi;

namespace SlackNet.Interaction
{
    public interface IMessageResponse
    {
        Message Message { get; set; }
    }

    public class MessageResponse : IMessageResponse
    {
        public ResponseType ResponseType { get; set; }
        public bool ReplaceOriginal { get; set; }
        public bool DeleteOriginal { get; set; }
        public Message Message { get; set; }
    }
}