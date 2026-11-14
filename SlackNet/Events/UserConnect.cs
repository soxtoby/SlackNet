namespace SlackNet.Events;

/// <summary>
/// Notifies the app that a user is requesting to connect with the app.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/user_connection/">Slack documentation</a> for more information.</remarks>
[SlackType("connect")]
public class UserConnect : UserConnection
{
    /// <summary>
    /// Allows the app to open a modal containing the steps, links, or buttons to get the user through its connection flow.
    /// </summary>
    public string TriggerId { get; set; }
}
