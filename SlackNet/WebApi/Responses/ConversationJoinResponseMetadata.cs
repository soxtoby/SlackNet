using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationJoinResponseMetadata : SlackObject
{
    public IList<string> Warnings { get; set; } = [];
}