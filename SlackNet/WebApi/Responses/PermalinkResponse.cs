namespace SlackNet.WebApi;

public class PermalinkResponse : SlackObject
{
    public string Channel { get; set; }
    public string Permalink { get; set; }
}