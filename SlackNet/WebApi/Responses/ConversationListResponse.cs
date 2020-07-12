using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class ConversationListResponse
    {
        public IList<Conversation> Channels { get; set; } = new List<Conversation>();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}