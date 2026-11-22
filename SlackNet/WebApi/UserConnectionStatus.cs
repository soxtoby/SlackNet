#nullable disable
namespace SlackNet.WebApi;

/// <summary>
/// The connection status between a user and an app.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/apps.user.connection.update/">Slack documentation</a> for more information.</remarks>
public enum UserConnectionStatus
{
    Connected,
    Disconnected
}
