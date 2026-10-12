namespace SlackNet.WebApi;

/// <summary>
/// The lifecycle status of an agent in an agent session.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.sessions.setStatus/">Slack documentation</a> for more information.</remarks>
public enum AgentSessionStatus
{
    /// <summary>
    /// The agent is ready for the next prompt or task.
    /// </summary>
    Active,

    /// <summary>
    /// The agent is working on a task. Slack shows a loading indicator, and a stop button if the app is subscribed to <see cref="Events.AgentSessionStopped"/>.
    /// </summary>
    Processing,

    /// <summary>
    /// The agent cannot make progress until the user intervenes, for example when it needs clarification, a decision, or a tool approval.
    /// An agent that has posted a question and is waiting on the answer is suspended, not active.
    /// </summary>
    Suspended,

    /// <summary>
    /// The agent has closed the session and will no longer respond on it.
    /// </summary>
    Closed
}