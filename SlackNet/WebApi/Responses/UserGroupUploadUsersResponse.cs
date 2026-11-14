#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class UserGroupUploadUsersResponse
{
    /// <summary>
    /// How many users were successfully added.
    /// </summary>
    public int SuccessfulUserCount { get; set; }

    /// <summary>
    /// Users that could not be added.
    /// </summary>
    public IList<UserGroupUserFailure> InvalidUsers { get; set; } = [];
}
