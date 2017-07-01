using SlackNet.Events;

namespace SlackNet.Rtm
{
    public class Reply : Event
    {
        public bool Ok { get; set; }
        public uint ReplyTo { get; set; }
        public string Ts { get; set; }
        public string Text { get; set; }
        public Error Error { get; set; }
    }
}