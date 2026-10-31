namespace SlackNet.Events;

/// <summary>
/// Sent when a user changes the title of an agent session, even if the agent previously set that title.
/// If your agent keeps sessions in sync between Slack and your own UI, reflect the user's title change accordingly.
/// To set a session's title from your app, use <c>agents.sessions.rename</c>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/agent_session_title_changed/">Slack documentation</a> for more information.</remarks>
public class AgentSessionTitleChanged : Event
{
    /// <summary>
    /// The channel containing the session.
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// The root of the thread the session is scoped to.
    /// </summary>
    public string ThreadTs { get; set; }

    /// <summary>
    /// The user who changed the title.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// The new title of the session.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The title of the session before the change. Omitted when the session had no title before the change.
    /// </summary>
    public string PreviousTitle { get; set; }
}
