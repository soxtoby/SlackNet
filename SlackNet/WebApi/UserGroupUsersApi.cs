using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IUserGroupUsersApi
    {
        /// <summary>
        /// Returns a list of all users within a User Group.
        /// </summary>
        /// <param name="userGroupId">ID of the User Group to update.</param>
        /// <param name="includeDisabled">Allow results that involve disabled User Groups.</param>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<string>> List(string userGroupId, bool includeDisabled = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Updates the list of users that belong to a User Group.
        /// This method replaces all users in a User Group with the list of users provided in the <paramref name="userIds"/> parameter.
        /// </summary>
        /// <param name="userGroupId">ID of the User Group to update.</param>
        /// <param name="userIds">User IDs that represent the entire list of users for the User Group.</param>
        /// <param name="includeCount">Include the number of users in the User Group.</param>
        /// <param name="cancellationToken"></param>
        Task<UserGroup> Update(string userGroupId, IEnumerable<string> userIds, bool includeCount = false, CancellationToken? cancellationToken = null);
    }

    public class UserGroupUsersApi : IUserGroupUsersApi
    {
        private readonly ISlackApiClient _client;
        public UserGroupUsersApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Returns a list of all users within a User Group.
        /// </summary>
        /// <param name="userGroupId">ID of the User Group to update.</param>
        /// <param name="includeDisabled">Allow results that involve disabled User Groups.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<string>> List(string userGroupId, bool includeDisabled = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<MembershipResponse>("usergroups.users.list", new Args
                {
                    { "usergroup", userGroupId },
                    { "include_disabled", includeDisabled }
                }, cancellationToken).ConfigureAwait(false))
            .Users;

        /// <summary>
        /// Updates the list of users that belong to a User Group.
        /// This method replaces all users in a User Group with the list of users provided in the <paramref name="userIds"/> parameter.
        /// </summary>
        /// <param name="userGroupId">ID of the User Group to update.</param>
        /// <param name="userIds">User IDs that represent the entire list of users for the User Group.</param>
        /// <param name="includeCount">Include the number of users in the User Group.</param>
        /// <param name="cancellationToken"></param>
        public async Task<UserGroup> Update(string userGroupId, IEnumerable<string> userIds, bool includeCount = false, CancellationToken? cancellationToken = null) =>
            (await _client.Post<UserGroupResponse>("usergroups.users.update", new Args
                {
                    { "usergroup", userGroupId },
                    { "users", userIds },
                    { "include_count", includeCount }
                }, cancellationToken).ConfigureAwait(false))
            .Usergroup;
    }
}