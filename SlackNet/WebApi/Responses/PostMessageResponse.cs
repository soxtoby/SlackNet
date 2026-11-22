using SlackNet.Events;

namespace SlackNet.WebApi;

public class PostMessageResponse
{
    public string Ts { get; set; } = null!;
    public string Channel { get; set; } = null!;
    public MessageEvent? Message { get; set; }
}