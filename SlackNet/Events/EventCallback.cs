using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Events
{
    /// <summary>
    /// Also referred to as the "outer event", or the JSON object containing the event that happened itself.
    /// </summary>
    public class EventCallback : EventRequest
    {
        public string TeamId { get; set; }
        public string ApiAppId { get; set; }
        public Event Event { get; set; }
        public IList<Authorization> Authorizations { get; set; }
        public string EventContext { get; set; }
        public string EventId { get; set; }
        public int EventTime { get; set; }
        [JsonIgnore]
        public DateTime EventDateTime => EventTime.ToDateTime().GetValueOrDefault();
    }
    
    /// <summary>
    /// Also referred to as the "outer event", or the JSON object containing the event that happened itself.
    /// </summary>
    public class EventCallback<T> : EventCallback where T : Event
    {
        public new T Event { get; set; }
    }
}

