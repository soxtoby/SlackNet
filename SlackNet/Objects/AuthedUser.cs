namespace SlackNet;

/// <summary>
/// If you make use of a <a href="https://docs.slack.dev/authentication/tokens/#user">user_token</a> this object is included.
/// </summary>
public class AuthedUser
{
    public string Id { get; set; }
    public string Scope { get; set; }
    public string AccessToken { get; set; }
#nullable enable

    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See the <a href="https://docs.slack.dev/authentication/using-token-rotation/">Slack documentation</a> for more information.
    /// </summary>
    public int? ExpiresIn { get; set; }
    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See the <a href="https://docs.slack.dev/authentication/using-token-rotation/">Slack documentation</a> for more information.
    /// </summary>
    public string? RefreshToken { get; set; }
#nullable disable
    public string TokenType { get; set; }
}