using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IPinsApi
{
    /// <summary>
    /// Pins a message to a particular channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/pins.add">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to pin the message in.</param>
    /// <param name="ts">Timestamp of the message to pin.</param>
    /// <param name="cancellationToken"></param>
    Task AddMessage(string channelId, string ts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the items pinned to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/pins.list">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to get pinned items for.</param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<PinnedItem>> List(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Un-pins a file from a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/pins.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel where the file is pinned to.</param>
    /// <param name="fileId">File to un-pin.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveFile(string channelId, string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Un-pins a file comment from a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/pins.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel where the file comment is pinned to.</param>
    /// <param name="fileCommentId">File comment to un-pin.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveFileComment(string channelId, string fileCommentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Un-pins a message from a channel.
    /// </summary>
    /// <remarks>See the <a href="https://api.slack.com/methods/pins.remove">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel where the message is pinned to.</param>
    /// <param name="ts">Timestamp of the message to un-pin.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveMessage(string channelId, string ts, CancellationToken cancellationToken = default);
}

public class PinsApi : IPinsApi
{
    private readonly ISlackApiClient _client;
    public PinsApi(ISlackApiClient client) => _client = client;

    public Task AddMessage(string channelId, string ts = null, CancellationToken cancellationToken = default) =>
        _client.Post("pins.add", new Args
            {
                { "channel", channelId },
                { "timestamp", ts }
            }, cancellationToken);

    public async Task<IReadOnlyList<PinnedItem>> List(string channelId, CancellationToken cancellationToken = default) =>
        (await _client.Get<PinnedItemListResponse>("pins.list", new Args { { "channel", channelId } }, cancellationToken).ConfigureAwait(false)).Items;

    public Task RemoveFile(string channelId, string fileId, CancellationToken cancellationToken = default) =>
        _client.Post("pins.remove", new Args
            {
                { "channel", channelId },
                { "file", fileId }
            }, cancellationToken);

    public Task RemoveFileComment(string channelId, string fileCommentId, CancellationToken cancellationToken = default) =>
        _client.Post("pins.remove", new Args
            {
                { "channel", channelId },
                { "file_comment", fileCommentId }
            }, cancellationToken);

    public Task RemoveMessage(string channelId, string ts, CancellationToken cancellationToken = default) =>
        _client.Post("pins.remove", new Args
            {
                { "channel", channelId },
                { "timestamp", ts }
            }, cancellationToken);
}