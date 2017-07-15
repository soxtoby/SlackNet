using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public class StarsApi
    {
        private readonly SlackApiClient _client;
        public StarsApi(SlackApiClient client) => _client = client;

        /// <summary>
        /// Adds a star to a file.
        /// </summary>
        /// <param name="fileId">File to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToFile(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.add", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToFileComment(string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.add", new Args { { "file_comment", fileCommentId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToChannel(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.add", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to add star to was posted.</param>
        /// <param name="ts">Timestamp of the message to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToMessage(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.add", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);

        /// <summary>
        /// Lists the items starred by the authed user.
        /// </summary>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<StarListResponse> List(int count = 100, int page = 1, CancellationToken? cancellationToken = null) =>
            _client.Get<StarListResponse>("stars.list", new Args
                {
                    { "count", count },
                    { "page", page }
                }, cancellationToken);

        /// <summary>
        /// Removes a star from a file.
        /// </summary>
        /// <param name="fileId">File to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFile(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.remove", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFileComment(string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.remove", new Args { { "file_comment", fileCommentId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromChannel(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.remove", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to remove star from was posted.</param>
        /// <param name="ts">Timestamp of the message to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromMessage(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("stars.remove", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);
    }
}