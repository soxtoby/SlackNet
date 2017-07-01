using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class ChannelListResponse
    {
        public List<Channel> Channels { get; } = new List<Channel>();
    }
}