namespace SlackNet.Events;

public class EventRequest : SlackObject
{
    public string Type { get; set; }
    public string Token { get; set; }
}