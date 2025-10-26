﻿using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IOpenIdApi
{
    /// <summary>
    /// Exchanges a temporary OAuth verifier code for an access token for Sign in with Slack.
    /// See https://docs.slack.dev/reference/methods/openid.connect.token for more information.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/openid.connect.token/">Slack documentation</a> for more information.</remarks>
    /// <param name="clientId">Issued when you created your application.</param>
    /// <param name="clientSecret">Issued when you created your application.</param>
    /// <param name="code">The code param returned via the OAuth callback.</param>
    /// <param name="grantType">The grant_type param as described in the OAuth spec.</param>
    /// <param name="redirectUri">This must match the originally submitted URI (if one was sent).</param>
    /// <param name="refreshToken">The refresh_token param as described in the OAuth spec.</param>
    /// <param name="cancellationToken"></param>
    Task<OpenIdTokenResponse> Token(
        string clientId,
        string clientSecret,
#nullable enable
        string? code,
        string? grantType,
        string? redirectUri,
        string? refreshToken,
#nullable disable
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Get the identity of a user who has authorized Sign in with Slack.
    /// See https://docs.slack.dev/reference/methods/openid.connect.userInfo for more information.
    /// Note: that this method uses an Authentication token passed in the header.
    /// Use <c>SlackApiClient.WithAccessToken</c> to specify a token.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/openid.connect.userInfo/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<OpenIdUserInfoResponse> UserInfo(CancellationToken cancellationToken);
}

public class OpenIdApi : IOpenIdApi
{
    private readonly ISlackApiClient _client;
    public OpenIdApi(ISlackApiClient client) => _client = client;

    public Task<OpenIdTokenResponse> Token(
        string clientId,
        string clientSecret,
#nullable enable
        string? code = null,
        string? grantType = null,
        string? redirectUri = null,
        string? refreshToken = null,
#nullable disable
        CancellationToken cancellationToken = default
    ) =>
        _client.Post<OpenIdTokenResponse>("openid.connect.token", new Args(), new SlackFormContent
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "code", code },
                    { "grant_type", grantType },
                    { "redirect_uri", redirectUri },
                    { "refresh_token", refreshToken },
                }
            , cancellationToken);

    public Task<OpenIdUserInfoResponse> UserInfo(CancellationToken cancellationToken = default) =>
        _client.Post<OpenIdUserInfoResponse>("openid.connect.userInfo", new Args(), cancellationToken);
}