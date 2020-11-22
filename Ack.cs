namespace SlackNet.Rtm
{
    public class Ack : OutgoingRtmEvent
    {
        public string envelope_id { get; set; }
    }
}