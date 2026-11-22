#nullable disable
namespace SlackNet.Events;

/// <summary>
/// A member's user connection status change requested.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/user_connection/">Slack documentation</a> for more information.</remarks>
public class UserConnection : Event
{
    /// <summary>
    /// Dictates what tasks an app must perform to satisfy the request from the end-user.
    /// Either <c>connect</c> (<see cref="UserConnect"/>) or <c>disconnect</c> (<see cref="UserDisconnect"/>).
    /// </summary>
    public string Subtype { get; set; }

    /// <summary>
    /// The <see cref="SlackNet.User.Id"/> of the user requesting to connect with or disconnect from the app.
    /// </summary>
    public string User { get; set; }
}
