#nullable disable
namespace SlackNet.Events;

/// <summary>
/// User has uninstalled an app.
/// Contains information about an app that has been uninstalled from a team or organization.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/app_uninstalled_team/">Slack documentation</a> for more information.</remarks>
public class AppUninstalledTeam : Event
{
    /// <summary>
    /// The app that was uninstalled from the team.
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// The name of the app that was uninstalled from the team.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The owner of the app that was uninstalled from the team.
    /// </summary>
    public string AppOwnerId { get; set; }

    /// <summary>
    /// The user that uninstalled the app from the team (if available).
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The team that the app was uninstalled from.
    /// </summary>
    public string TeamId { get; set; }

    /// <summary>
    /// The domain of the team that the app was uninstalled from.
    /// </summary>
    public string TeamDomain { get; set; }
}
