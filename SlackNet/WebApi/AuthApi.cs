using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IAuthApi
{
    /// <summary>
    /// Revokes an access token. Use it when you no longer need a token.
    /// For example, with a Sign In With Slack app, call this to log a user out.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/auth.revoke">Slack documentation</a> for more information.</remarks>
    /// <param name="test">Setting this parameter to True triggers a testing mode where the specified token will not actually be revoked.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether or not the access token was revoked.</returns>
    Task<bool> Revoke(bool test, CancellationToken cancellationToken);

    /// <summary>
    /// Checks authentication and tells you who you are.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/auth.test">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<AuthTestResponse> Test(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain a full list of workspaces your org-wide app has been approved for.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/auth.teams.list">Slack documentation</a> for more information.</remarks>
    /// <param name="cursor">Set cursor to next_cursor returned by the previous call to list items in the next page.</param>
    /// <param name="includeIcon">Whether to return icon paths for each workspace. An icon path represents a URI pointing to the image signifying the workspace.</param>
    /// <param name="limit">The maximum number of workspaces to return. Must be a positive integer no larger than 1000.</param>
    /// <param name="cancellationToken"></param>
    Task<AuthTeamsListResponse> TeamsList(string cursor = null, bool includeIcon = false, int limit = 100, CancellationToken cancellationToken = default);
}

public class AuthApi(ISlackApiClient client) : IAuthApi
{
    public async Task<bool> Revoke(bool test, CancellationToken cancellationToken = default) =>
        (await client.Get<RevokeResponse>("auth.revoke", new Args { { "test", test } }, cancellationToken).ConfigureAwait(false)).Revoked;

    public Task<AuthTestResponse> Test(CancellationToken cancellationToken = default) =>
        client.Post<AuthTestResponse>("auth.test", new Args(), cancellationToken);

    public Task<AuthTeamsListResponse> TeamsList(string cursor = null, bool includeIcon = false, int limit = 100, CancellationToken cancellationToken = default) =>
        client.Get<AuthTeamsListResponse>("auth.teams.list", new Args
            {
                { "cursor", cursor },
                { "include_icon", includeIcon },
                { "limit", limit }
            }, cancellationToken);
}