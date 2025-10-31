#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IAssistantSearchApi
{
    /// <summary>
    /// Returns search capabilities on a given team.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/assistant.search.info/">Slack documentation</a> for more information.</remarks>
    /// <param name="cancellationToken"></param>
    Task<AssistantSearchInfoResponse> Info(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches messages across your Slack organization—perfect for broad, specific, and real-time data retrieval.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/assistant.search.context/">Slack documentation</a> for more information.</remarks>
    /// <param name="query">User prompt or search query.</param>
    /// <param name="actionToken">Send <see cref="MessageEvent.ActionToken"/> as received in a <see cref="MessageEvent"/>.</param>
    /// <param name="channelTypes">Mix and match channel types by providing a list of any combination of channel types.</param>
    /// <param name="contentTypes">Content types to include.</param>
    /// <param name="includeBots">If you want the results to include bots.</param>
    /// <param name="contextChannelId">Context channel ID to support scoping the search when applicable.</param>
    /// <param name="cursor">The cursor returned by the API. Leave this blank for the first request, and use this to get the next page of results.</param>
    /// <param name="limit">Number of results to return, up to a max of 20.</param>
    /// <param name="cancellationToken"></param>
    Task<AssistantSearchContextResponse> Context(
        string query,
        string? actionToken = null,
        IEnumerable<ConversationType>? channelTypes = null,
        IEnumerable<ContentType>? contentTypes = null,
        bool includeBots = false,
        string? contextChannelId = null,
        string? cursor = null,
        int limit = 20,
        CancellationToken cancellationToken = default
    );
}

public class AssistantSearchApi(ISlackApiClient client) : IAssistantSearchApi
{
    public Task<AssistantSearchInfoResponse> Info(CancellationToken cancellationToken = default) =>
        client.Post<AssistantSearchInfoResponse>("assistant.search.info", new Args(), cancellationToken);

    public Task<AssistantSearchContextResponse> Context(
        string query,
        string? actionToken = null,
        IEnumerable<ConversationType>? channelTypes = null,
        IEnumerable<ContentType>? contentTypes = null,
        bool includeBots = false,
        string? contextChannelId = null,
        string? cursor = null,
        int limit = 20,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<AssistantSearchContextResponse>("assistant.search.context",
            new Args
                {
                    { "query", query },
                    { "action_token", actionToken },
                    { "channel_types", channelTypes },
                    { "content_types", contentTypes },
                    { "include_bots", includeBots },
                    { "context_channel_id", contextChannelId },
                    { "cursor", cursor },
                    { "limit", limit }
                },
            cancellationToken);
}