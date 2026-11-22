#nullable disable
namespace SlackNet.Events;

/// <summary>
/// User has deleted an app.
/// Contains information about an app that has been deleted from a team or organization it belongs to.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/app_deleted/">Slack documentation</a> for more information.</remarks>
public class AppDeleted : Event
{
    /// <summary>
    /// The app that was deleted from the team.
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// The name of the app that was deleted from the team.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The owner of the app that was deleted from the team.
    /// </summary>
    public string AppOwnerId { get; set; }

    /// <summary>
    /// The team that the app was deleted from.
    /// </summary>
    public string TeamId { get; set; }

    /// <summary>
    /// The domain of the team that the app was deleted from.
    /// </summary>
    public string TeamDomain { get; set; }
}
