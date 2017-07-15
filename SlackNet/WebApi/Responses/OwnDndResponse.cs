using System;

namespace SlackNet.WebApi
{
    public class OwnDndResponse : DndResponse
    {
        public bool SnoozeEnabled { get; set; }
        public int SnoozeEndTime { get; set; }
        public DateTime? SnoozeEnd => SnoozeEndTime.ToDateTime();
        public int SnoozeRemaining { get; set; }
        public TimeSpan SnoozeRemainingTimeSpan => TimeSpan.FromSeconds(SnoozeRemaining);
    }
}