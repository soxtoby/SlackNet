namespace SlackNet.WebApi;

public class AuthTestResponse : SlackObject
{
    public string Url { get; set; }
    public string Team { get; set; }
    public string User { get; set; }
    public string TeamId { get; set; }
    public string UserId { get; set; }

#nullable enable
    /// <summary>
    /// Only available when authenticating with a bot token. Identifies the app installation, which is distinct from <see cref="UserId"/>. See the <a href="https://docs.slack.dev/reference/methods/auth.test">Slack documentation</a> for more information.
    /// </summary>
    public string? BotId { get; set; }
    /// <summary>
    /// Only available when the token belongs to a team within an Enterprise Grid organization. See the <a href="https://docs.slack.dev/reference/methods/auth.test">Slack documentation</a> for more information.
    /// </summary>
    public string? EnterpriseId { get; set; }
}