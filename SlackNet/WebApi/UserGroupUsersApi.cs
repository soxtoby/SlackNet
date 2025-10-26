﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IUserGroupUsersApi
{
    /// <summary>
    /// Returns a list of all users within a User Group.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/usergroups.users.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the User Group to update.</param>
    /// <param name="includeDisabled">Allow results that involve disabled User Groups.</param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<string>> List(string userGroupId, bool includeDisabled = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the list of users that belong to a User Group.
    /// This method replaces all users in a User Group with the list of users provided in the <paramref name="userIds"/> parameter.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/usergroups.users.update/">Slack documentation</a> for more information.</remarks>
    /// <param name="userGroupId">ID of the User Group to update.</param>
    /// <param name="userIds">User IDs that represent the entire list of users for the User Group.</param>
    /// <param name="includeCount">Include the number of users in the User Group.</param>
    /// <param name="cancellationToken"></param>
    Task<UserGroup> Update(string userGroupId, IEnumerable<string> userIds, bool includeCount = false, CancellationToken cancellationToken = default);
}

public class UserGroupUsersApi(ISlackApiClient client) : IUserGroupUsersApi
{
    public async Task<IReadOnlyList<string>> List(string userGroupId, bool includeDisabled = false, CancellationToken cancellationToken = default) =>
        (await client.Get<MembershipResponse>("usergroups.users.list", new Args
            {
                { "usergroup", userGroupId },
                { "include_disabled", includeDisabled }
            }, cancellationToken).ConfigureAwait(false))
        .Users;

    public async Task<UserGroup> Update(string userGroupId, IEnumerable<string> userIds, bool includeCount = false, CancellationToken cancellationToken = default) =>
        (await client.Post<UserGroupResponse>("usergroups.users.update", new Args
            {
                { "usergroup", userGroupId },
                { "users", userIds },
                { "include_count", includeCount }
            }, cancellationToken).ConfigureAwait(false))
        .Usergroup;
}