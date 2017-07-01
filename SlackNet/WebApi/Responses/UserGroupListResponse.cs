using System.Collections.Generic;
using SlackNet.Objects;

namespace SlackNet.WebApi.Responses
{
    class UserGroupListResponse
    {
        public List<UserGroup> Usergroups { get; } = new List<UserGroup>();
    }
}