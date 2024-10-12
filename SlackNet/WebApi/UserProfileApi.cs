using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IUserProfileApi
{
    /// <summary>
    /// Use this method to retrieve a user's profile information.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/users.profile.get">Slack documentation</a> for more information.</remarks>
    /// <param name="includeLabels">Include labels for each ID in custom profile fields.</param>
    /// <param name="userId">User to retrieve profile info for (defaults to authed user).</param>
    /// <param name="cancellationToken"></param>
    Task<UserProfile> Get(bool includeLabels = false, string userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/users.profile.set">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Name of a single key to set.</param>
    /// <param name="value">Value to set a single key to.</param>
    /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
    /// <param name="cancellationToken"></param>
    Task<UserProfile> Set(string name, string value, string userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Use this method to set a user's profile information, including name, email, current status, and other attributes.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/users.profile.set">Slack documentation</a> for more information.</remarks>
    /// <param name="profile">Changes to user's profile. Null properties will not be changed.</param>
    /// <param name="userId">ID of user to change (defaults to authed user). This argument may only be specified by team admins on paid teams.</param>
    /// <param name="cancellationToken"></param>
    Task<UserProfile> Set(UserProfile profile, string userId = null, CancellationToken cancellationToken = default);
}

public class UserProfileApi : IUserProfileApi
{
    private readonly ISlackApiClient _client;
    public UserProfileApi(ISlackApiClient client) => _client = client;

    public async Task<UserProfile> Get(bool includeLabels = false, string userId = null, CancellationToken cancellationToken = default) =>
        (await _client.Get<UserProfileResponse>("users.profile.get", new Args
            {
                { "include_labels", includeLabels },
                { "user", userId }
            }, cancellationToken).ConfigureAwait(false))
        .Profile;

    public async Task<UserProfile> Set(string name, string value, string userId = null, CancellationToken cancellationToken = default) =>
        (await _client.Post<UserProfileResponse>("users.profile.set", new Args
            {
                { "name", name },
                { "value", value },
                { "user", userId }
            }, cancellationToken).ConfigureAwait(false))
        .Profile;

    public async Task<UserProfile> Set(UserProfile profile, string userId = null, CancellationToken cancellationToken = default) =>
        (await _client.Post<UserProfileResponse>("users.profile.set", new Args
            {
                { "profile", profile },
                { "user", userId }
            }, cancellationToken).ConfigureAwait(false))
        .Profile;
}