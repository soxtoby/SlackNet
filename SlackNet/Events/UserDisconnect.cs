#nullable disable
namespace SlackNet.Events;

/// <summary>
/// Informs the app that a user is requesting to disconnect from the app.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/user_connection/">Slack documentation</a> for more information.</remarks>
[SlackType("disconnect")]
public class UserDisconnect : UserConnection
{
    /// <summary>
    /// The ID of the user's enterprise organization.
    /// </summary>
    public string EnterpriseId { get; set; }
}
