using System.Collections.Generic;

namespace SlackNet.WebApi;

record UserGroupAddUsersResponse
{
    public IList<UserGroupUserFailure> InvalidUsers { get; init; } = [];
}
