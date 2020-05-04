using SlackNet.Events;

namespace SlackNet.Interaction
{
    [SlackType("message_action")]
    public class MessageShortcut : InteractionRequest
    {
        public string CallbackId { get; set; }
        public string TriggerId { get; set; }
        public MessageEvent Message { get; set; }
    }
}