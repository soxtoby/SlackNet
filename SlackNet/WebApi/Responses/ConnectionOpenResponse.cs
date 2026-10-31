namespace SlackNet.WebApi;

public class ConnectionOpenResponse : SlackObject
{
    public bool Ok { get; set; }
    public string Url { get; set; }
}