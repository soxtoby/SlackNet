using System.Collections.Generic;

namespace SlackNet.WebApi;

public class MessageActivityResponse
{
    public IList<MessageActivity> MessageActivities { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
