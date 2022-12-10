namespace SlackNet.Rtm;

public class Typing : OutgoingRtmEvent
{
    public string Channel { get; set; }
}