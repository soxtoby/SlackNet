﻿using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IScheduledMessagesApi
{
    /// <summary>
    /// Returns a list of pending scheduled messages.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/chat.scheduledMessages.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">The channel of the scheduled messages.</param>
    /// <param name="latestTs">The latest value in the time range.</param>
    /// <param name="oldestTs">The oldest value in the time range.</param>
    /// <param name="limit">Maximum number of original entries to return.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ScheduledMessageListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ScheduledMessageListResponse> List(
        string channelId = null,
        string latestTs = null,
        string oldestTs = null,
        int limit = 100,
        string cursor = null,
        CancellationToken cancellationToken = default);
}

public class ScheduledMessagesApi(ISlackApiClient client) : IScheduledMessagesApi
{
    public Task<ScheduledMessageListResponse> List(
        string channelId = null,
        string latestTs = null,
        string oldestTs = null,
        int limit = 100,
        string cursor = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<ScheduledMessageListResponse>("chat.scheduledMessages.list", new Args
            {
                { "channel", channelId },
                { "latest", latestTs },
                { "oldest", oldestTs },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
}