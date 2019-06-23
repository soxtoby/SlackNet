using SlackNet.Events;

namespace SlackNet.Interaction
{
    public class MessageAction : InteractionRequest
    {
        public string CallbackId { get; set; }
        public string TriggerId { get; set; }
        public MessageEvent Message { get; set; }
    }
}