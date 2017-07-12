using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class GroupListResponse
    {
        public List<Channel> Groups { get; } = new List<Channel>();
    }
}