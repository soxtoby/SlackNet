using SlackNet.Objects;
using SlackNet.Rtm;

namespace SlackNet.WebApi.Responses
{
    public class ConnectResponse
    {
        public string Url { get; set; }
        public Team Team { get; set; }
        public Self Self { get; set; }
    }
}