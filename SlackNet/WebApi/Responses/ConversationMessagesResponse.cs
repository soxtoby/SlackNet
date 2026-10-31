using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi;

public class ConversationMessagesResponse : SlackObject
{
    public IList<MessageEvent> Messages { get; set; } = [];
    public bool HasMore { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}