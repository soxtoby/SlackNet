using System;
using Newtonsoft.Json;

namespace SlackNet.Events
{
    /// <summary>
    /// Also referred to as the "outer event", or the JSON object containing the event that happened itself.
    /// </summary>
    public class EventCallback : Event
    {
        public string Token { get; set; }
        public string TeamId { get; set; }
        public string ApiAppId { get; set; }
        public Event Event { get; set; }
        public string[] AuthedUsers { get; set; }
        public string EventId { get; set; }
        public int EventTime { get; set; }
        [JsonIgnore]
        public DateTime EventDateTime => EventTime.ToDateTime().GetValueOrDefault();
    }
}

