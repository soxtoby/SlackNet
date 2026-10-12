using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IAgentsSessionsApi
{
    /// <summary>
    /// Sets the lifecycle status of an agent session, creating the session if it doesn't exist yet.
    /// Only apps declared as agents in the app settings can create sessions.
    /// While a session is <see cref="AgentSessionStatus.Processing"/>, Slack shows a loading indicator,
    /// along with a stop button if the app subscribes to <see cref="Events.AgentSessionStopped"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/agents.sessions.setStatus/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">ID of the channel containing the agent session. The app must be a member of it. Slack requires this for public channels.</param>
    /// <param name="threadTs">
    /// Timestamp of the thread root message the session is scoped to.
    /// Required for thread-based sessions in regular channels and DMs; must be <c>null</c> for session channels (where the whole channel is one session).
    /// </param>
    /// <param name="status">The lifecycle status to set.</param>
    /// <param name="title">Title for the agent session (max 200 characters). Only used when the session is created; use <c>agents.sessions.rename</c> to rename an existing session.</param>
    /// <param name="initiatorUserId">The user who initiated the session. Only used when the session is created. Must be a member of the channel.</param>
    /// <param name="iconEmoji">
    /// Emoji to use as the agent's icon. Takes priority over <paramref name="iconUrl"/>. Requires the <c>chat:write.customize</c> scope.
    /// Icon and username overrides persist until replaced: a call that sets none of <paramref name="iconEmoji"/>, <paramref name="iconUrl"/> and <paramref name="username"/> leaves them in place,
    /// while a call that sets any of them replaces all three, so those it leaves out are cleared.
    /// </param>
    /// <param name="iconUrl">URL to an image to use as the agent's icon. Requires the <c>chat:write.customize</c> scope.</param>
    /// <param name="username">Display name override for the agent (max 200 characters). Requires the <c>chat:write.customize</c> scope.</param>
    /// <param name="cancellationToken"></param>
    Task<AgentSessionStatusResponse> SetStatus(
        string channelId,
        [CanBeNull] string threadTs,
        AgentSessionStatus status,
        [CanBeNull] string title = null,
        [CanBeNull] string initiatorUserId = null,
        [CanBeNull] string iconEmoji = null,
        [CanBeNull] string iconUrl = null,
        [CanBeNull] string username = null,
        CancellationToken cancellationToken = default);
}

public class AgentsSessionsApi(ISlackApiClient client) : IAgentsSessionsApi
{
    public Task<AgentSessionStatusResponse> SetStatus(
        string channelId,
        string threadTs,
        AgentSessionStatus status,
        string title = null,
        string initiatorUserId = null,
        string iconEmoji = null,
        string iconUrl = null,
        string username = null,
        CancellationToken cancellationToken = default) =>
        client.Post<AgentSessionStatusResponse>("agents.sessions.setStatus", new Args
            {
                { "channel_id", channelId },
                { "thread_ts", threadTs },
                { "status", status },
                { "title", title },
                { "initiator_user_id", initiatorUserId },
                { "icon_emoji", iconEmoji },
                { "icon_url", iconUrl },
                { "username", username }
            }, cancellationToken);
}