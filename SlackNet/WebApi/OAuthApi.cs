using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IOAuthApi
    {
        /// <summary>
        /// Allows you to exchange a temporary OAuth code for an API access token. This is used as part of the OAuth authentication flow.
        /// </summary>
        /// <remarks>See the <a href="https://api.slack.com/methods/oauth.access">Slack documentation</a> for more information.</remarks>
        /// <param name="clientId">Issued when you created your application.</param>
        /// <param name="clientSecret">Issued when you created your application.</param>
        /// <param name="code">The code param returned via the OAuth callback.</param>
        /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
        /// <param name="cancellationToken"></param>
        Task<OauthAccessResponse> Access(string clientId, string clientSecret, string code, string redirectUrl, CancellationToken? cancellationToken = null);
    }

    public class OAuthApi : IOAuthApi
    {
        private readonly ISlackApiClient _client;
        public OAuthApi(ISlackApiClient client) => _client = client;

        public Task<OauthAccessResponse> Access(string clientId, string clientSecret, string code, string redirectUrl, CancellationToken? cancellationToken = null) =>
            _client.Get<OauthAccessResponse>("oauth.access", new Args
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "code", code },
                    { "redirect_uri", redirectUrl }
                }, cancellationToken);
    }
}