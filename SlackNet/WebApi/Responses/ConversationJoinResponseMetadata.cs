using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationJoinResponseMetadata
{
    public IList<string> Warnings { get; set; } = new List<string>();
}