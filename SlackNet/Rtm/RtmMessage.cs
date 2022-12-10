namespace SlackNet.Rtm;

public class RtmMessage : OutgoingRtmEvent
{
    public string Channel { get; set; }
    public string Text { get; set; }
}