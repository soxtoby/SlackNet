using SlackNet.Rtm;

namespace SlackNet.WebApi;

public class ConnectResponse : SlackObject
{
    public string Url { get; set; }
    public Team Team { get; set; }
    public Self Self { get; set; }
}