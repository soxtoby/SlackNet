using System.Collections.Generic;

namespace SlackNet.Events;

/// <summary>
/// Sent when a user clicks the native stop button shown while an agent session is <see cref="WebApi.AgentSessionStatus.Processing"/>.
/// The button is only shown if the app is subscribed to this event, which requires the <c>chat:write</c> scope.
/// When you receive it, stop any in-progress work for the channel and thread, confirm to the user that work has stopped,
/// and move the session out of <see cref="WebApi.AgentSessionStatus.Processing"/> with <see cref="WebApi.IAgentsSessionsApi.SetStatus"/> or the <c>sessionStatus</c> argument of <see cref="WebApi.IChatApi.StopStream"/>.
/// The session status is not updated automatically.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/agent_session_stopped/">Slack documentation</a> for more information.</remarks>
public class AgentSessionStopped : Event
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
    /// The user who clicked the stop button.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Timestamps of the app's in-progress streaming messages that Slack stopped in response to the click.
    /// Empty when no stream was active, so the event still tells you to stop work that isn't a stream.
    /// </summary>
    public IList<string> StreamingMessageTs { get; set; } = [];
}