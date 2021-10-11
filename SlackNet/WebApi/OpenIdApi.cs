using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IOpenIdApi
    {
        /// <summary>
        /// Exchanges a temporary OAuth verifier code for an access token for Sign in with Slack.
        /// See https://api.slack.com/methods/openid.connect.token for more information.
        /// </summary>
        /// <param name="clientId">Issued when you created your application.</param>
        /// <param name="clientSecret">Issued when you created your application.</param>
        /// <param name="code">The code param returned via the OAuth callback.</param>
        /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
        /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
        /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
        /// <param name="cancellationToken"></param>
        Task<OpenIdTokenResponse> Token(
            string clientId,
            string clientSecret,
#nullable enable
            string? code,
            string? grantType,
            string? redirectUrl,
            string? refreshToken,
#nullable disable
            CancellationToken? cancellationToken
        );

        /// <summary>
        /// Get the identity of a user who has authorized Sign in with Slack.
        /// See https://api.slack.com/methods/openid.connect.userInfo for more information.
        /// Note: that this method uses an Authentication token passed in the header.
        /// Use <c>SlackApiClient.WithAccessToken</c> to specify a token.
        /// </summary>
        /// <param name="token">Authentication token bearing required scopes. Tokens should be passed as an HTTP Authorization header or alternatively, as a POST parameter.</param>
        /// <param name="cancellationToken"></param>
        Task<OpenIdUserInfoResponse> UserInfo(CancellationToken? cancellationToken);
  }

    public class OpenIdApi : IOpenIdApi
    {
        private readonly ISlackApiClient _client;
        public OpenIdApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Exchanges a temporary OAuth verifier code for an access token for Sign in with Slack.
        /// See https://api.slack.com/methods/openid.connect.token for more information.
        /// </summary>
        /// <param name="clientId">Issued when you created your application.</param>
        /// <param name="clientSecret">Issued when you created your application.</param>
        /// <param name="code">The code param returned via the OAuth callback.</param>
        /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
        /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
        /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
        /// <param name="cancellationToken"></param>
        public Task<OpenIdTokenResponse> Token(
            string clientId,
            string clientSecret,
#nullable enable
            string? code = null,
            string? grantType = null,
            string? redirectUrl = null,
            string? refreshToken = null,
#nullable disable
            CancellationToken? cancellationToken = null)
        {
            var form = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };
            if (!string.IsNullOrEmpty(code)) form.Add("code", code);
            if (!string.IsNullOrEmpty(grantType)) form.Add("grant_type", grantType);
            if (!string.IsNullOrEmpty(redirectUrl)) form.Add("redirect_url", redirectUrl);
            if (!string.IsNullOrEmpty(refreshToken)) form.Add("refresh_token", refreshToken);


            return _client.Post<OpenIdTokenResponse>("openid.connect.token", new Args { }, new FormUrlEncodedContent(form), cancellationToken);
        }


        /// <summary>
        /// Get the identity of a user who has authorized Sign in with Slack.
        /// See https://api.slack.com/methods/openid.connect.userInfo for more information.
        /// Note: that this method uses an Authentication token passed in the header.
        /// Use <c>SlackApiClient.WithAccessToken</c> to specify a token.
        /// </summary>
        /// <param name="token">Authentication token bearing required scopes. Tokens should be passed as an HTTP Authorization header or alternatively, as a POST parameter.</param>
        /// <param name="cancellationToken"></param>
        public Task<OpenIdUserInfoResponse> UserInfo(CancellationToken? cancellationToken = null) =>
            _client.Post<OpenIdUserInfoResponse>("openid.connect.userInfo", new Args { }, cancellationToken);
    }
}
