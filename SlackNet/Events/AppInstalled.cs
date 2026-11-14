namespace SlackNet.Events;

/// <summary>
/// User has installed an app.
/// Contains information about an app that has been installed on a team or organization.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/app_installed/">Slack documentation</a> for more information.</remarks>
public class AppInstalled : Event
{
    /// <summary>
    /// The app that was installed on the team.
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// The name of the app that was installed on the team.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The owner of the app that was installed on the team.
    /// </summary>
    public string AppOwnerId { get; set; }

    /// <summary>
    /// The user that installed the app on the team.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// The team that the app was installed on.
    /// </summary>
    public string TeamId { get; set; }

    /// <summary>
    /// The domain of the team that the app was installed on.
    /// </summary>
    public string TeamDomain { get; set; }
}
