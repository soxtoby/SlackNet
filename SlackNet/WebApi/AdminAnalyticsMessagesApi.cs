using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Retrieves analytics for messages in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAnalyticsMessagesApi
{
    /// <summary>
    /// Retrieves activity metrics for messages from a given channel, created between <paramref name="oldestTs"/> and <paramref name="latestTs"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.messages.activity/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID for channel of the message activity to query.</param>
    /// <param name="oldestTs">
    /// Oldest timestamp to include in the results. Defaults to 7 days before current time.
    /// If not passed while still passing <paramref name="latestTs"/>, defaults to 7 days before <paramref name="latestTs"/>.
    /// </param>
    /// <param name="latestTs">
    /// Most recent timestamp to include in results. Defaults to current time.
    /// If not passed while still passing <paramref name="oldestTs"/>, defaults to 7 days after <paramref name="oldestTs"/>.
    /// </param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="MessageActivityResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">Maximum number of entries to return. Defaults to 50.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageActivityResponse> Activity(
        string channelId,
        string? oldestTs = null,
        string? latestTs = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves metadata for a list of messages from a given channel.
    /// This returns structural information about messages without the actual message content,
    /// including character counts, reactions, file attachments, and threading information.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.analytics.messages.metadata/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel ID for channel containing the messages to query.</param>
    /// <param name="oldestTs">Oldest timestamp to include in the results.</param>
    /// <param name="latestTs">Most recent timestamp to include in the results. If not passed, defaults to current time.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AnalyticsMessagesResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<AnalyticsMessagesResponse> Metadata(
        string channelId,
        string? oldestTs = null,
        string? latestTs = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);
}

public class AdminAnalyticsMessagesApi(ISlackApiClient client) : IAdminAnalyticsMessagesApi
{
    public Task<MessageActivityResponse> Activity(
        string channelId,
        string? oldestTs = null,
        string? latestTs = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<MessageActivityResponse>("admin.analytics.messages.activity", new Args
            {
                { "channel", channelId },
                { "oldest_ts", oldestTs },
                { "latest_ts", latestTs },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task<AnalyticsMessagesResponse> Metadata(
        string channelId,
        string? oldestTs = null,
        string? latestTs = null,
        string? cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<AnalyticsMessagesResponse>("admin.analytics.messages.metadata", new Args
            {
                { "channel", channelId },
                { "oldest_ts", oldestTs },
                { "latest_ts", latestTs },
                { "cursor", cursor }
            }, cancellationToken);
}
