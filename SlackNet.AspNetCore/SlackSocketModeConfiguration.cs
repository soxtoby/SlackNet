using SlackNet.SocketMode;

namespace SlackNet.AspNetCore;

class SlackSocketModeConfiguration
{
    public bool SocketMode { get; set; }
    public SocketModeConnectionOptions ConnectionOptions { get; set; }
}
