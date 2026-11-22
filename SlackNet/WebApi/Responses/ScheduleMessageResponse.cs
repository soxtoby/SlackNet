using SlackNet.Events;

namespace SlackNet.WebApi;

public class ScheduleMessageResponse
{
    public string Channel { get; set; } = null!;
    public string ScheduledMessageId { get; set; } = null!;
    public string PostAt { get; set; } = null!;
    public MessageEvent Message { get; set; } = null!;
}