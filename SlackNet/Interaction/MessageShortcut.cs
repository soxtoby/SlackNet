using System;
using SlackNet.Events;

namespace SlackNet.Interaction
{
    [Obsolete("Use MessageShortcut instead")]
    public class MessageAction : InteractionRequest
    {
        public string CallbackId { get; set; }
        public string TriggerId { get; set; }
        public MessageEvent Message { get; set; }
    }

    [SlackType("message_action")]
    public class MessageShortcut : MessageAction { }
}