using SlackNet.Rtm;

namespace SlackNet.WebApi;

public class ConnectResponse
{
    public string Url { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public Self Self { get; set; } = null!;
}