using System.Collections.Generic;

namespace SlackNet.WebApi;

record UserGroupAddUsersResponse
{
    public IList<UserFailure> InvalidUsers { get; init; } = [];
}
