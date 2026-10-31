namespace SlackNet.WebApi;

public class MessageTsResponse : SlackObject
{
    public string Channel { get; set; }
    public string Ts { get; set; }
}