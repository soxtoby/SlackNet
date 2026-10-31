using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi;

public class AssistantSearchContextResponse : SlackObject
{
    public AssistantSearchContextMessages Result { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}

public class AssistantSearchContextMessages : SlackObject
{
    public IList<MessageEvent> Messages { get; set; } = [];
}