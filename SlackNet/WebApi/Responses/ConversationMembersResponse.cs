using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class ConversationMembersResponse
    {
        public IList<string> Members { get; set; } = new List<string>();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }
}