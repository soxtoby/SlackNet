using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IUserProfileApi
    {
        /// <summary>
        /// Use this method to retrieve a user's profile information.
        /// </summary>
        /// <param name="includeLabels">Include labels for each ID in custom profile fields.</param>
        /// <param name="userId">User to retrieve profile info for (defaults to authed user).</param>
        /// <param name="cancellationToken"></param>
        Task<UserProfile> Get(bool includeLabels = false, string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
        /// </summary>
        /// <param name="name">Name of a single key to set.</param>
        /// <param name="value">Value to set a single key to.</param>
        /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
        /// <param name="cancellationToken"></param>
        Task<UserProfile> Set(string name, string value, string userId = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
        /// </summary>
        /// <param name="profile">Changes to user's profile. Null properties will not be changed.</param>
        /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
        /// <param name="cancellationToken"></param>
        Task<UserProfile> Set(UserProfile profile, string userId = null, CancellationToken? cancellationToken = null);
    }

    public class UserProfileApi : IUserProfileApi
    {
        private readonly ISlackApiClient _client;
        public UserProfileApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Use this method to retrieve a user's profile information.
        /// </summary>
        /// <param name="includeLabels">Include labels for each ID in custom profile fields.</param>
        /// <param name="userId">User to retrieve profile info for (defaults to authed user).</param>
        /// <param name="cancellationToken"></param>
        public async Task<UserProfile> Get(bool includeLabels = false, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Get<UserProfileResponse>("users.profile.get", new Args
                {
                    { "include_labels", includeLabels },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Profile;

        /// <summary>
        /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
        /// </summary>
        /// <param name="name">Name of a single key to set.</param>
        /// <param name="value">Value to set a single key to.</param>
        /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
        /// <param name="cancellationToken"></param>
        public async Task<UserProfile> Set(string name, string value, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Post<UserProfileResponse>("users.profile.set", new Args
                {
                    { "name", name },
                    { "value", value },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Profile;

        /// <summary>
        /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
        /// </summary>
        /// <param name="profile">Changes to user's profile. Null properties will not be changed.</param>
        /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
        /// <param name="cancellationToken"></param>
        public async Task<UserProfile> Set(UserProfile profile, string userId = null, CancellationToken? cancellationToken = null) =>
            (await _client.Post<UserProfileResponse>("users.profile.set", new Args
                {
                    { "profile", profile },
                    { "user", userId }
                }, cancellationToken).ConfigureAwait(false))
            .Profile;
    }
}