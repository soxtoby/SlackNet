using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class ScheduledMessageListResponse
{
    public IList<ScheduledMessage> ScheduledMessages { get; set; } = [];
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

public class ScheduledMessage
{
    public string Id { get; set; }
    public string ChannelId { get; set; }
    public int PostAt { get; set; }
    [JsonIgnore]
    public DateTime PostAtDate => PostAt.ToDateTime().GetValueOrDefault();
    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
    public string Text { get; set; }
}