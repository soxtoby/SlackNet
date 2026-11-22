#nullable disable
namespace SlackNet.Events;

/// <summary>
/// User requested an app.
/// Contains info about an app that a user on a team has requested to install.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/app_requested/">Slack documentation</a> for more information.</remarks>
public class AppRequested : Event
{
    public AppRequest AppRequest { get; set; }
}
