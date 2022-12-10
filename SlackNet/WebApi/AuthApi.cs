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
    Task<bool> Revoke(bool test, CancellationToken? cancellationToken);

    /// <summary>
    /// Checks authentication and tells you who you are.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/auth.test">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<AuthTestResponse> Test(CancellationToken? cancellationToken = null);
}

public class AuthApi : IAuthApi
{
    private readonly ISlackApiClient _client;
    public AuthApi(ISlackApiClient client) => _client = client;

    public async Task<bool> Revoke(bool test, CancellationToken? cancellationToken = null) =>
        (await _client.Get<RevokeResponse>("auth.revoke", new Args { { "test", test } }, cancellationToken).ConfigureAwait(false)).Revoked;

    public Task<AuthTestResponse> Test(CancellationToken? cancellationToken = null) =>
        _client.Post<AuthTestResponse>("auth.test", new Args(), cancellationToken);
}