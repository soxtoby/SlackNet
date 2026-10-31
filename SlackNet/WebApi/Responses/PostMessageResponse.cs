using SlackNet.Events;

namespace SlackNet.WebApi;

public class PostMessageResponse : SlackObject
{
    public string Ts { get; set; }
    public string Channel { get; set; }
    public MessageEvent Message { get; set; }
}