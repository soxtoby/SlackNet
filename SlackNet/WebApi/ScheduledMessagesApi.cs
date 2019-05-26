using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IScheduledMessagesApi
    {
        /// <summary>
        /// Returns a list of pending scheduled messages.
        /// </summary>
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
            CancellationToken? cancellationToken = null);
    }

    public class ScheduledMessagesApi : IScheduledMessagesApi
    {
        private readonly ISlackApiClient _client;
        public ScheduledMessagesApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Returns a list of pending scheduled messages.
        /// </summary>
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
        public Task<ScheduledMessageListResponse> List(
            string channelId = null,
            string latestTs = null,
            string oldestTs = null,
            int limit = 100,
            string cursor = null,
            CancellationToken? cancellationToken = null
        ) =>
            _client.Get<ScheduledMessageListResponse>("chat.scheduledMessages.list", new Args
                {
                    { "channel", channelId },
                    { "latest", latestTs },
                    { "oldest", oldestTs },
                    { "limit", limit },
                    { "cursor", cursor }
                }, cancellationToken);
    }
}