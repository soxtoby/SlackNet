using System;

namespace SlackNet.Objects
{
    public class Reminder
    {
        public string Id { get; set; }
        public string Creator { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public bool Recurring { get; set; }
        public int Time { get; set; }
        public DateTime? DateTime => Time.ToDateTime();
        public int CompleteTs { get; set; }
        public DateTime? Completed => CompleteTs.ToDateTime();
    }
}