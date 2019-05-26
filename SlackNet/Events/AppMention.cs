using Newtonsoft.Json;
using System;

namespace SlackNet.Events
{
    /// <summary>
    /// Subscribe to only the message events that mention your app or bot.
    /// </summary>
    public class AppMention : Event
    {
        public string User { get; set; }
        public string Text { get; set; }
        public string Ts { get; set; }
        [JsonIgnore]
        public DateTime Timestamp => Ts.ToDateTime().GetValueOrDefault();
        public string Channel { get; set; }
        public string EventTs { get; set; }
    }
}