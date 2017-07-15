using System.Collections.Generic;

namespace SlackNet.WebApi
{
    class UserGroupListResponse
    {
        public List<UserGroup> Usergroups { get; } = new List<UserGroup>();
    }
}