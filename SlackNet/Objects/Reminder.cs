using System;
using Newtonsoft.Json;

namespace SlackNet
{
    public class Reminder
    {
        public string Id { get; set; }
        public string Creator { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public bool Recurring { get; set; }
        public int Time { get; set; }
        [JsonIgnore]
        public DateTime? DateTime => Time.ToDateTime();
        public int CompleteTs { get; set; }
        [JsonIgnore]
        public DateTime? Completed => CompleteTs.ToDateTime();
    }
}