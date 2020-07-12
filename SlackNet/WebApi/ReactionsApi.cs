using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IReactionsApi
    {
        /// <summary>
        /// Adds a reaction (emoji) to a message.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="channelId">Channel where the message to add reaction to was posted.</param>
        /// <param name="ts">Timestamp of the message to add reaction to.</param>
        /// <param name="cancellationToken"></param>
        Task AddToMessage(string name, string channelId, string ts, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all reactions for a single file.
        /// </summary>
        /// <param name="fileId">File to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        Task<File> GetForFile(string fileId, bool full = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all reactions for a single file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        Task<FileComment> GetForFileComment(string fileCommentId, bool full = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all reactions for a single message.
        /// </summary>
        /// <param name="channelId">Channel where the message to get reactions for was posted.</param>
        /// <param name="ts">Timestamp of the message to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        Task<MessageEvent> GetForMessage(string channelId, string ts, bool full = false, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Returns a list of all items (file, file comment, channel message, group message, or direct message) reacted to by a user.
        /// </summary>
        /// <param name="userId">Show reactions made by this user. Defaults to the authed user.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cursor">
        /// Parameter for pagination.
        /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="ReactionItemListResponse.ResponseMetadata"/>. 
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<ReactionItemListResponse> List(string userId = null, bool full = false, int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a reaction (emoji) from a file.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="fileId">File to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromFile(string name, string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a reaction (emoji) from a file comment.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="fileCommentId">File comment to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromFileComment(string name, string fileCommentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Removes a reaction (emoji) from a message.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="channelId">Channel where the message to remove reaction from was posted.</param>
        /// <param name="ts">Timestamp of the message to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFromMessage(string name, string channelId, string ts, CancellationToken? cancellationToken = null);
    }

    public class ReactionsApi : IReactionsApi
    {
        private readonly ISlackApiClient _client;
        public ReactionsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Adds a reaction (emoji) to a message.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="channelId">Channel where the message to add reaction to was posted.</param>
        /// <param name="ts">Timestamp of the message to add reaction to.</param>
        /// <param name="cancellationToken"></param>
        public Task AddToMessage(string name, string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Post("reactions.add", new Args
                {
                    { "name", name },
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);

        /// <summary>
        /// Returns a list of all reactions for a single file.
        /// </summary>
        /// <param name="fileId">File to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        public async Task<File> GetForFile(string fileId, bool full = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<FileResponse>("reactions.get", new Args
                {
                    { "file", fileId },
                    { "full", full }
                }, cancellationToken).ConfigureAwait(false))
            .File;

        /// <summary>
        /// Returns a list of all reactions for a single file comment.
        /// </summary>
        /// <param name="fileCommentId">File comment to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        public async Task<FileComment> GetForFileComment(string fileCommentId, bool full = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<FileCommentReactionsResponse>("reactions.get", new Args
                {
                    { "file_comment", fileCommentId },
                    { "full", full }
                }, cancellationToken).ConfigureAwait(false))
            .Comment;

        /// <summary>
        /// Returns a list of all reactions for a single message.
        /// </summary>
        /// <param name="channelId">Channel where the message to get reactions for was posted.</param>
        /// <param name="ts">Timestamp of the message to get reactions for.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="cancellationToken"></param>
        public async Task<MessageEvent> GetForMessage(string channelId, string ts, bool full = false, CancellationToken? cancellationToken = null) =>
            (await _client.Get<MessageReactionsResponse>("reactions.get", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts },
                    { "full", full }
                }, cancellationToken).ConfigureAwait(false))
            .Message;

        /// <summary>
        /// Returns a list of all items (file, file comment, channel message, group message, or direct message) reacted to by a user.
        /// </summary>
        /// <param name="userId">Show reactions made by this user. Defaults to the authed user.</param>
        /// <param name="full">If true always return the complete reaction list.</param>
        /// <param name="count">Number of items to return per page.</param>
        /// <param name="page">Page number of results to return.</param>
        /// <param name="cursor">
        /// Parameter for pagination.
        /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="ReactionItemListResponse.ResponseMetadata"/>. 
        /// </param>
        /// <param name="cancellationToken"></param>
        public Task<ReactionItemListResponse> List(string userId = null, bool full = false, int count = 100, int page = 1, string cursor = null, CancellationToken? cancellationToken = null) =>
            _client.Get<ReactionItemListResponse>("reactions.list", new Args
                {
                    { "user", userId },
                    { "full", full },
                    { "count", count },
                    { "page", page },
                    { "cursor", cursor }
                }, cancellationToken);

        /// <summary>
        /// Removes a reaction (emoji) from a file.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="fileId">File to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFile(string name, string fileId, CancellationToken? cancellationToken = null) =>
            _client.Post("reactions.remove", new Args
                {
                    { "name", name },
                    { "file", fileId }
                }, cancellationToken);

        /// <summary>
        /// Removes a reaction (emoji) from a file comment.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="fileCommentId">File comment to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromFileComment(string name, string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Post("reactions.remove", new Args
                {
                    { "name", name },
                    { "file_comment", fileCommentId }
                }, cancellationToken);

        /// <summary>
        /// Removes a reaction (emoji) from a message.
        /// </summary>
        /// <param name="name">Reaction (emoji) name.</param>
        /// <param name="channelId">Channel where the message to remove reaction from was posted.</param>
        /// <param name="ts">Timestamp of the message to remove reaction from.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFromMessage(string name, string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Post("reactions.remove", new Args
                {
                    { "name", name },
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);
    }
}