#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// Who can join a workspace.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.settings.setDiscoverability/">Slack documentation</a> for more information.</remarks>
public enum TeamDiscoverability
{
    Open,
    InviteOnly,
    Closed,
    Unlisted
}
