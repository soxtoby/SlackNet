using System.Threading;
using System.Threading.Tasks;
using SlackNet.WebApi.Responses;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class AuthApi
    {
        private readonly SlackApiClient _client;
        public AuthApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Revokes an access token. Use it when you no longer need a token.
        /// For example, with a Sign In With Slack app, call this to log a user out.
        /// </summary>
        /// <param name="test">Setting this parameter to True triggers a testing mode where the specified token will not actually be revoked.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether or not the access token was revoked.</returns>
        public async Task<bool> Revoke(bool test, CancellationToken? cancellationToken) =>
            (await _client.Get<RevokeResponse>("auth.revoke", new Args { { "test", test } }, cancellationToken).ConfigureAwait(false)).Revoked;

        /// <summary>
        /// Checks authentication and tells you who you are.
        /// </summary>
        public Task<AuthTestResponse> Test(CancellationToken? cancellationToken = null) =>
            _client.Get<AuthTestResponse>("auth.test", new Args(), cancellationToken);
    }
}