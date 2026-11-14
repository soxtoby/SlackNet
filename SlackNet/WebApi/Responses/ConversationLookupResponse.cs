#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ConversationLookupResponse
{
    public IList<string> ChannelIds { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
