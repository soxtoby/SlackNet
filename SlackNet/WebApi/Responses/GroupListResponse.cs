using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class GroupListResponse
    {
        public List<Group> Groups { get; } = new List<Group>();
    }
}