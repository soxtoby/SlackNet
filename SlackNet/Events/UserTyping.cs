namespace SlackNet.Events
{
    public class UserTyping : Event
    {
        public string Channel { get; set; }
        public string User { get; set; }
    }
}