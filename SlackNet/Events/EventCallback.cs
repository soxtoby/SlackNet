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
        /// <summary>
        /// Will be deprecated by Slack on February 24, 2021.
        /// See <a href="https://api.slack.com/changelog/2020-09-15-events-api-truncate-authed-users">Slack's changelog page</a> for more information.
        /// </summary>
        [Obsolete("Use Authorizations and AppsEventsAuthorizations.List instead")]
        public string[] AuthedUsers { get; set; }
        public IList<Authorization> Authorizations { get; set; }
        public string EventContext { get; set; }
        public string EventId { get; set; }
        public int EventTime { get; set; }
        [JsonIgnore]
        public DateTime EventDateTime => EventTime.ToDateTime().GetValueOrDefault();
    }
}

