#nullable disable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IOAuthV2Api
{
    /// <summary>
    /// Exchanges a temporary OAuth verifier code for an access token.
    /// See https://docs.slack.dev/reference/methods/oauth.v2.access for more information.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/oauth.v2.access/">Slack documentation</a> for more information.</remarks>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="code">The code param returned via the OAuth callback.</param>
    /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
    /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
    /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
    /// <param name="codeVerifier">The code_verifier param as described in the PKCE extension to the OAuth spec, matching the code_challenge sent to the authorization endpoint.</param>
    /// <param name="cancellationToken"></param>
    Task<OauthV2AccessResponse> Access(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
        string? codeVerifier = null,
#nullable disable
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Exchanges a temporary OAuth verifier code for a user access token.
    /// </summary>
    /// <remarks>
    /// This method is used to initiate the OAuth flow using just user scopes (no bot scopes). It should be used in scenarios where:
    /// <list type="bullet">
    /// <item>You only need user tokens, not bot tokens</item>
    /// <item>You're building <a href="https://docs.slack.dev/ai/slack-mcp-server/">MCP integrations</a> with desktop IDE clients like Cursor or Claude Code</item>
    /// </list>
    /// See the <a href="https://docs.slack.dev/reference/methods/oauth.v2.user.access/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="code">The code param returned via the OAuth callback.</param>
    /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
    /// <param name="redirectUrl">This must match the originally submitted URI (if one was sent).</param>
    /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
    /// <param name="codeVerifier">The code_verifier param used to generate the code_challenge originally. Used for PKCE.</param>
    /// <param name="assertion">Identity assertion JWT authorization grant.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User access token and app installation information.</returns>
    Task<OauthV2AccessResponse> UserAccess(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
        string? codeVerifier = null,
        string? assertion = null,
#nullable disable
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Exchanges a legacy access token for a new expiring access token and refresh token.
    /// </summary>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="token">The legacy xoxb or xoxp token being migrated to use token rotation.</param>
    /// <param name="cancellationToken"></param>
    Task<OauthV2AccessResponse> Exchange(
        string clientId,
        string clientSecret,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Call this to rotate the secret on an old API token with a short secret.
    /// Must be paired with a call to <see cref="CompleteShortTokenRotation"/> to confirm the change.
    /// </summary>
    /// <remarks>
    /// Use this API method alongside the <see cref="CompleteShortTokenRotation"/> method to rotate a short-secret user token created before August 19, 2016 to a more secure token with a longer 32-character secret.
    /// This change does not take effect until you complete the flow. Do not overwrite your original token until the flow is complete.
    /// See the <a href="https://docs.slack.dev/reference/methods/oauth.v2.beginShortTokenRotation/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="clientId">Issued when you created your application. Must be the app the token being rotated was issued to.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="token">The xoxp token being rotated.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A new xoxp access token to replace the token with a shorter secret. Not yet active until <see cref="CompleteShortTokenRotation"/> is called.</returns>
    Task<string> BeginShortTokenRotation(
        string clientId,
        string clientSecret,
        string token,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Call this to finish rotating the secret on an old API token with a short secret.
    /// Must be paired with a call to <see cref="BeginShortTokenRotation"/> to start the process.
    /// </summary>
    /// <remarks>
    /// Use this API method alongside the <see cref="BeginShortTokenRotation"/> method to rotate a short-secret user token created before August 19, 2016 to a more secure token with a longer 32-character secret.
    /// If the response to this API call is successful, the original token will be invalidated and the new token will go live. The new token will be exactly the same as the original, but with a longer secret.
    /// If you do not get a successful response from <see cref="CompleteShortTokenRotation"/>, the rotation may not have been completed. Call <see cref="IAuthApi.Test"/> with the original and new token to check which is valid.
    /// See the <a href="https://docs.slack.dev/reference/methods/oauth.v2.completeShortTokenRotation/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="clientId">Issued when you created your application. Must be the app the token being rotated was issued to.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="token">The original short xoxp token being rotated (the one passed to <see cref="BeginShortTokenRotation"/>). Authenticates the request.</param>
    /// <param name="newToken">The new xoxp token returned by <see cref="BeginShortTokenRotation"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The new xoxp access token, now active. The previous token is no longer valid.</returns>
    Task<string> CompleteShortTokenRotation(
        string clientId,
        string clientSecret,
        string token,
        string newToken,
        CancellationToken cancellationToken = default
    );
}

public class OAuthV2Api(ISlackApiClient client) : IOAuthV2Api
{
    public Task<OauthV2AccessResponse> Access(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
        string? codeVerifier = null,
#nullable disable
        CancellationToken cancellationToken = default
    ) =>
        client.WithAccessToken(string.Empty) // Since this endpoint is for getting an access token, it doesn't make sense to include an existing token in the request
            .Post<OauthV2AccessResponse>("oauth.v2.access", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "code", code },
                        { "grant_type", grantType },
                        { "redirect_uri", redirectUrl },
                        { "refresh_token", refreshToken },
                        { "code_verifier", codeVerifier }
                    },
                cancellationToken);

    public Task<OauthV2AccessResponse> UserAccess(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUrl,
        string? refreshToken,
        string? codeVerifier = null,
        string? assertion = null,
#nullable disable
        CancellationToken cancellationToken = default
    ) =>
        client.WithAccessToken(string.Empty) // Since this endpoint is for getting an access token, it doesn't make sense to include an existing token in the request
            .Post<OauthV2AccessResponse>("oauth.v2.user.access", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "code", code },
                        { "grant_type", grantType },
                        { "redirect_uri", redirectUrl },
                        { "refresh_token", refreshToken },
                        { "code_verifier", codeVerifier },
                        { "assertion", assertion }
                    },
                cancellationToken);

    public Task<OauthV2AccessResponse> Exchange(string clientId, string clientSecret, string token, CancellationToken cancellationToken = default) =>
        client.WithAccessToken(string.Empty) // Since this endpoint is for exchanging an access token, it doesn't make sense to include an existing token in the request
            .Post<OauthV2AccessResponse>("oauth.v2.exchange", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "token", token }
                    },
                cancellationToken);

    public async Task<string> BeginShortTokenRotation(string clientId, string clientSecret, string token, CancellationToken cancellationToken = default) =>
        (await client.WithAccessToken(string.Empty) // The token being rotated is sent with the client credentials, rather than the client's own token
            .Post<BeginShortTokenRotationResponse>("oauth.v2.beginShortTokenRotation", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "token", token }
                    },
                cancellationToken).ConfigureAwait(false))
        .NewToken;

    public async Task<string> CompleteShortTokenRotation(string clientId, string clientSecret, string token, string newToken, CancellationToken cancellationToken = default) =>
        (await client.WithAccessToken(string.Empty) // The token being rotated is sent with the client credentials, rather than the client's own token
            .Post<CompleteShortTokenRotationResponse>("oauth.v2.completeShortTokenRotation", new Args(), new SlackFormContent
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "token", token },
                        { "new_token", newToken }
                    },
                cancellationToken).ConfigureAwait(false))
        .Token;
}