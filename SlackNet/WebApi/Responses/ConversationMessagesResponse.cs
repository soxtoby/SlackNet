using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi
{
    public class ConversationMessagesResponse
    {
        public IList<MessageEvent> Messages { get; set; } = new List<MessageEvent>();
        public bool HasMore { get; set; }
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}