using SlackNet.Events;

namespace SlackNet.WebApi
{
    public class ScheduleMessageResponse
    {
        public string Channel { get; set; }
        public string ScheduledMessageId { get; set; }
        public string PostAt { get; set; }
        public MessageEvent Message { get; set; }
    }
}