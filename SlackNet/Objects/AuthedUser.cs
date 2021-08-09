namespace SlackNet
{
  /// <summary>
  /// If you make use of a <a href="https://api.slack.com/authentication/token-types#user">user_token</a> this object is included.
  /// </summary>
  public class AuthedUser
  {
    public string Id { get; set; }
    public string Scope { get; set; }
    public string AccessToken { get; set; }
#nullable enable

    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See more: https://api.slack.com/authentication/rotation
    /// </summary>
    public int? ExpiresIn { get; set; }
    /// <summary>
    /// Only available for OAuth flows with Token Rotation enabled. See more: https://api.slack.com/authentication/rotation
    /// </summary>
    public string? RefreshToken { get; set; }
#nullable disable
    public string TokenType { get; set; }
  }
}
