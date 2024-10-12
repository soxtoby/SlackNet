using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IOAuthV2Api
{
    /// <summary>
    /// Exchanges a temporary OAuth verifier code for an access token.
    /// See https://api.slack.com/methods/oauth.v2.access for more information.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/oauth.v2.access">Slack documentation</a> for more information.</remarks>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="code">The code param returned via the OAuth callback.</param>
    /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
    /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
    /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
    /// <param name="cancellationToken"></param>
    Task<OauthV2AccessResponse> Access(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
#nullable disable
        CancellationToken cancellationToken
    );

}

public class OAuthV2Api : IOAuthV2Api
{
    private readonly ISlackApiClient _client;
    public OAuthV2Api(ISlackApiClient client) => _client = client;

    public Task<OauthV2AccessResponse> Access(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
#nullable disable
        CancellationToken cancellationToken = default
    ) =>
        _client.WithAccessToken(string.Empty) // Since this endpoint is for getting an access token, it doesn't make sense to include an existing token in the request
            .Post<OauthV2AccessResponse>("oauth.v2.access", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "code", code },
                        { "grant_type", grantType },
                        { "redirect_uri", redirectUrl },
                        { "refresh_token", refreshToken }
                    },
                cancellationToken);
}