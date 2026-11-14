#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationTeamsResponse
{
    public IList<string> TeamIds { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
