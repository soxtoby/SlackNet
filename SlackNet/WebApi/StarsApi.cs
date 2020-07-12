using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IStarsApi
    {
        /// <summary>
        /// Adds a star to a file.
        /// </summary>
        /// <param name="fileId">File to add star to.</param>
        /// <param name="cancellationToken"></param>
        Task AddToFile(string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Adds a star to a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to add star to.</param>
        /// <param name="cancellationToken"></param>
        Task AddToFileComment(string fileCommentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Adds a star to a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to add star to.</param>
        /// <param name="cancellationToken"></param>
        Task AddToChannel(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Adds a star to a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to add star to was posted.</param>
        /// <param name="ts">Timestamp of the message to add star to.</param>
        /// <param name="cancellationToken"></param>
        Task AddToMessage(string channelId, string ts, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lists the items starred by the authed user.
        /// </summary>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cursor">
        /// Parameter for pagination.
        /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="StarListResponse.ResponseMetadata"/>.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<StarListResponse> List(int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a star from a file.
        /// </summary>
        /// <param name="fileId">File to remove star from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromFile(string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a star from a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to remove star from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromFileComment(string fileCommentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a star from a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to remove star from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromChannel(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a star from a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to remove star from was posted.</param>
        /// <param name="ts">Timestamp of the message to remove star from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromMessage(string channelId, string ts, CancellationToken? cancellationToken = null);
    }

    public class StarsApi : IStarsApi
    {
        private readonly ISlackApiClient _client;
        public StarsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Adds a star to a file.
        /// </summary>
        /// <param name="fileId">File to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToFile(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.add", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToFileComment(string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.add", new Args { { "file_comment", fileCommentId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToChannel(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.add", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Adds a star to a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to add star to was posted.</param>
        /// <param name="ts">Timestamp of the message to add star to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToMessage(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.add", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);

        /// <summary>
        /// Lists the items starred by the authed user.
        /// </summary>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cursor">
        /// Parameter for pagination.
        /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="StarListResponse.ResponseMetadata"/>.
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<StarListResponse> List(int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<StarListResponse>("stars.list", new Args
                {
                    { "count", count },
                    { "page", page },
                    { "cursor", cursor }
                }, cancellationToken);

        /// <summary>
        /// Removes a star from a file.
        /// </summary>
        /// <param name="fileId">File to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFile(string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.remove", new Args { { "file", fileId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFileComment(string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.remove", new Args { { "file_comment", fileCommentId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a channel.
        /// </summary>
        /// <param name="channelId">Channel, private group, or DM to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromChannel(string channelId, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.remove", new Args { { "channel", channelId } }, cancellationToken);

        /// <summary>
        /// Removes a star from a message.
        /// </summary>
        /// <param name="channelId">Channel where the message to remove star from was posted.</param>
        /// <param name="ts">Timestamp of the message to remove star from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromMessage(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Post("stars.remove", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);
    }
}