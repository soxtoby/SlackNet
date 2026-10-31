using System.Collections.Generic;

namespace SlackNet.WebApi;

public class ScheduledMessageListResponse : SlackObject
{
    public IList<ScheduledMessage> ScheduledMessages { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

public class ScheduledMessage : SlackObject
{
    public string Id { get; set; }
    public string ChannelId { get; set; }
    public int PostAt { get; set; }
    public int DateCreated { get; set; }
    public string Text { get; set; }
}