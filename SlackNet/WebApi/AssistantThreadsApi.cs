using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IAssistantThreadsApi
{
    /// <summary>
    /// Set the status for an AI assistant thread.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/assistant.threads.setStatus/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID containing the assistant thread.</param>
    /// <param name="threadTs">Message timestamp of the thread of where to set the status.</param>
    /// <param name="status">Status of the specified bot user, e.g. "is thinking..."</param>
    /// <param name="loadingMessages">The list of messages to rotate through as a loading indicator.</param>
    /// <param name="cancellationToken"></param>
    Task SetStatus(string channelId, string threadTs, string status, [CanBeNull] IEnumerable<string> loadingMessages = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set suggested prompts for the given assistant thread.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/assistant.threads.setSuggestedPrompts/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID containing the assistant thread.</param>
    /// <param name="threadTs">Message timestamp of the thread to set suggested prompts for.</param>
    /// <param name="prompts">Each prompt should be supplied with its <see cref="AssistantPrompt.Title"/> and <see cref="AssistantPrompt.Message"/> properties.</param>
    /// <param name="title">Title for the list of provided prompts. For example: Suggested Prompts, Related Questions.</param>
    /// <param name="cancellationToken"></param>
    Task SetSuggestedPrompts(string channelId, string threadTs, IEnumerable<AssistantPrompt> prompts, [CanBeNull] string title = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Set the title for the given assistant thread.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/assistant.threads.setTitle/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID containing the assistant thread.</param>
    /// <param name="threadTs">Message timestamp of the thread to set suggested prompts for.</param>
    /// <param name="title">The title to use for the thread.</param>
    /// <param name="cancellationToken"></param>
    Task SetTitle(string channelId, string threadTs, string title, CancellationToken cancellationToken = default);
}

public class AssistantThreadsApi(ISlackApiClient client) : IAssistantThreadsApi
{
    public Task SetStatus(string channelId, string threadTs, string status, IEnumerable<string> loadingMessages = null, CancellationToken cancellationToken = default) =>
        client.Post("assistant.threads.setStatus", new Args
            {
                { "channel_id", channelId },
                { "thread_ts", threadTs },
                { "status", status },
                { "loading_messages", loadingMessages }
            }, cancellationToken);

    public Task SetSuggestedPrompts(string channelId, string threadTs, IEnumerable<AssistantPrompt> prompts, string title = null, CancellationToken cancellationToken = default) =>
        client.Post("assistant.threads.setSuggestedPrompts", new Args
            {
                { "channel_id", channelId },
                { "thread_ts", threadTs },
                { "prompts", prompts },
                { "title", title }
            }, cancellationToken);

    public Task SetTitle(string channelId, string threadTs, string title, CancellationToken cancellationToken = default) =>
        client.Post("assistant.threads.setTitle", new Args
            {
                { "channel_id", channelId },
                { "thread_ts", threadTs },
                { "title", title }
            }, cancellationToken);
}