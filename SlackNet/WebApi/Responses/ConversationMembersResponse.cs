using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationMembersResponse : SlackObject
{
    public IList<string> Members { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}