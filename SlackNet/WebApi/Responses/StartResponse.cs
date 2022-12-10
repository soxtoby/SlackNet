using System.Collections.Generic;

namespace SlackNet.WebApi;

public class StartResponse : ConnectResponse
{
    public IList<User> Users { get; set; }
    public IList<Channel> Channels { get; set; }
    public IList<Channel> Groups { get; set; }
    public IList<Channel> Mpims { get; set; }
    public IList<Im> Ims { get; set; }
    public IList<BotInfo> Bots { get; set; }
}