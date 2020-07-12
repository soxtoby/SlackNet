using System.Collections.Generic;

namespace SlackNet.WebApi
{
    public class ScheduledMessageListResponse
    {
        public IList<ScheduledMessage> ScheduledMessages { get; set; } = new List<ScheduledMessage>();
        public ResponseMetadata ResponseMetadata { get; set; } = new ResponseMetadata();
    }

    public class ScheduledMessage
    {
        public int Id { get; set; }
        public string ChannelId { get; set; }
        public int PostAt { get; set; }
        public int DateCreated { get; set; }
    }
}