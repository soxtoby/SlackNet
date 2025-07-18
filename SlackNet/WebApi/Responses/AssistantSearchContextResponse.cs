using System.Collections.Generic;
using SlackNet.Events;

namespace SlackNet.WebApi;

public class AssistantSearchContextResponse
{
    public AssistantSearchContextMessages Result { get; set; }
    public ResponseMetadata ResponseMetadata { get; set; }
}

public class AssistantSearchContextMessages
{
    public IList<MessageEvent> Messages { get; set; } = [];
}