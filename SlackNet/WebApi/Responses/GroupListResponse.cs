using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class GroupListResponse
    {
        public List<Channel> Groups { get; } = new List<Channel>();
    }
}