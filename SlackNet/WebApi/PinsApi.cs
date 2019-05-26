using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi
{
    public interface IPinsApi
    {
        /// <summary>
        /// Pins a file to a particular channel.
        /// Deprecated: will no longer accept files or file comments beginning August 22, 2019
        /// </summary>
        /// <param name="channelId">Channel to pin the file in.</param>
        /// <param name="fileId">File to pin.</param>
        /// <param name="cancellationToken"></param>
        [Obsolete]
        Task AddFile(string channelId, string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Pins a file comment to a particular channel.
        /// Deprecated: will no longer accept files or file comments beginning August 22, 2019
        /// </summary>
        /// <param name="channelId">Channel to pin the file comment in.</param>
        /// <param name="fileCommentId">File comment to pin.</param>
        /// <param name="cancellationToken"></param>
        [Obsolete]
        Task AddFileComment(string channelId, string fileCommentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Pins a message to a particular channel.
        /// </summary>
        /// <param name="channelId">Channel to pin the message in.</param>
        /// <param name="ts">Timestamp of the message to pin.</param>
        /// <param name="cancellationToken"></param>
        Task AddMessage(string channelId, string ts = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Lists the items pinned to a channel.
        /// </summary>
        /// <param name="channelId">Channel to get pinned items for.</param>
        /// <param name="cancellationToken"></param>
        Task<IReadOnlyList<PinnedItem>> List(string channelId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Un-pins a file from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the file is pinned to.</param>
        /// <param name="fileId">File to un-pin.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFile(string channelId, string fileId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Un-pins a file comment from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the file comment is pinned to.</param>
        /// <param name="fileCommentId">File comment to un-pin.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveFileComment(string channelId, string fileCommentId, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Un-pins a message from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the message is pinned to.</param>
        /// <param name="ts">Timestamp of the message to un-pin.</param>
        /// <param name="cancellationToken"></param>
        Task RemoveMessage(string channelId, string ts, CancellationToken? cancellationToken = null);
    }

    public class PinsApi : IPinsApi
    {
        private readonly ISlackApiClient _client;
        public PinsApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Pins a file to a particular channel.
        /// </summary>
        /// <param name="channelId">Channel to pin the file in.</param>
        /// <param name="fileId">File to pin.</param>
        /// <param name="cancellationToken"></param>
        public Task AddFile(string channelId, string fileId, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.add", new Args
                {
                    { "channel", channelId },
                    { "file", fileId }
                }, cancellationToken);

        /// <summary>
        /// Pins a file comment to a particular channel.
        /// </summary>
        /// <param name="channelId">Channel to pin the file comment in.</param>
        /// <param name="fileCommentId">File comment to pin.</param>
        /// <param name="cancellationToken"></param>
        public Task AddFileComment(string channelId, string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.add", new Args
                {
                    { "channel", channelId },
                    { "file_comment", fileCommentId },
                }, cancellationToken);

        /// <summary>
        /// Pins a message to a particular channel.
        /// </summary>
        /// <param name="channelId">Channel to pin the message in.</param>
        /// <param name="ts">Timestamp of the message to pin.</param>
        /// <param name="cancellationToken"></param>
        public Task AddMessage(string channelId, string ts = null, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.add", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);

        /// <summary>
        /// Lists the items pinned to a channel.
        /// </summary>
        /// <param name="channelId">Channel to get pinned items for.</param>
        /// <param name="cancellationToken"></param>
        public async Task<IReadOnlyList<PinnedItem>> List(string channelId, CancellationToken? cancellationToken = null) =>
            (await _client.Get<PinnedItemListResponse>("pins.list", new Args { { "channel", channelId } }, cancellationToken).ConfigureAwait(false)).Items;

        /// <summary>
        /// Un-pins a file from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the file is pinned to.</param>
        /// <param name="fileId">File to un-pin.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFile(string channelId, string fileId, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.remove", new Args
                {
                    { "channel", channelId },
                    { "file", fileId }
                }, cancellationToken);

        /// <summary>
        /// Un-pins a file comment from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the file comment is pinned to.</param>
        /// <param name="fileCommentId">File comment to un-pin.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveFileComment(string channelId, string fileCommentId, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.remove", new Args
                {
                    { "channel", channelId },
                    { "file_comment", fileCommentId }
                }, cancellationToken);

        /// <summary>
        /// Un-pins a message from a channel.
        /// </summary>
        /// <param name="channelId">Channel where the message is pinned to.</param>
        /// <param name="ts">Timestamp of the message to un-pin.</param>
        /// <param name="cancellationToken"></param>
        public Task RemoveMessage(string channelId, string ts, CancellationToken? cancellationToken = null) =>
            _client.Get("pins.remove", new Args
                {
                    { "channel", channelId },
                    { "timestamp", ts }
                }, cancellationToken);
    }
}