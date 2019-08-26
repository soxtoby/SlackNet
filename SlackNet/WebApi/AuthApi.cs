using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IAuthApi
    {
        /// <summary>
        /// Revokes an access token. Use it when you no longer need a token.
        /// For example, with a Sign In With Slack app, call this to log a user out.
        /// </summary>
        /// <param name="test">Setting this parameter to True triggers a testing mode where the specified token will not actually be revoked.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether or not the access token was revoked.</returns>
        Task<bool> Revoke(bool test, CancellationToken? cancellationToken);

        /// <summary>
        /// Checks authentication and tells you who you are.
        /// </summary>
        Task<AuthTestResponse> Test(CancellationToken? cancellationToken = null);
    }

    public class AuthApi : IAuthApi
    {
        private readonly ISlackApiClient _client;
        public AuthApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Revokes an access token. Use it when you no longer need a token.
        /// For example, with a Sign In With Slack app, call this to log a user out.
        /// </summary>
        /// <param name="test">Setting this parameter to True triggers a testing mode where the specified token will not actually be revoked.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Whether or not the access token was revoked.</returns>
        public async Task<bool> Revoke(bool test, CancellationToken? cancellationToken = null) =>
            (await _client.Get<RevokeResponse>("auth.revoke", new Args { { "test", test } }, cancellationToken).ConfigureAwait(false)).Revoked;

        /// <summary>
        /// Checks authentication and tells you who you are.
        /// </summary>
        public Task<AuthTestResponse> Test(CancellationToken? cancellationToken = null) =>
            _client.Post<AuthTestResponse>("auth.test", new Args(), cancellationToken);
    }
}