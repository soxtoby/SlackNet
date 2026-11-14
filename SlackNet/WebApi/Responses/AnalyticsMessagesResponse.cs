#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class AnalyticsMessagesResponse
{
    public IList<AnalyticsMessage> Messages { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
