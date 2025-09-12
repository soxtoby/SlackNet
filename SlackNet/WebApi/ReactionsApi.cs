using System.Threading;
using System.Threading.Tasks;
using SlackNet.Events;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IReactionsApi
{
    /// <summary>
    /// Adds a reaction (emoji) to a message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.add/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Reaction (emoji) name.</param>
    /// <param name="channelId">Channel where the message to add reaction to was posted.</param>
    /// <param name="ts">Timestamp of the message to add reaction to.</param>
    /// <param name="cancellationToken"></param>
    Task AddToMessage(string name, string channelId, string ts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of all reactions for a single file.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.get/">Slack documentation</a> for more information.</remarks>
    /// <param name="fileId">File to get reactions for.</param>
    /// <param name="full">If true always return the complete reaction list.</param>
    /// <param name="cancellationToken"></param>
    Task<File> GetForFile(string fileId, bool full = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of all reactions for a single file comment.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.get/">Slack documentation</a> for more information.</remarks>
    /// <param name="fileCommentId">File comment to get reactions for.</param>
    /// <param name="full">If true always return the complete reaction list.</param>
    /// <param name="cancellationToken"></param>
    Task<FileComment> GetForFileComment(string fileCommentId, bool full = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of all reactions for a single message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.get/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel where the message to get reactions for was posted.</param>
    /// <param name="ts">Timestamp of the message to get reactions for.</param>
    /// <param name="full">If true always return the complete reaction list.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageEvent> GetForMessage(string channelId, string ts, bool full = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of all items (file, file comment, channel message, group message, or direct message) reacted to by a user.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="userId">Show reactions made by this user. Defaults to the authed user.</param>
    /// <param name="full">If true always return the complete reaction list.</param>
    /// <param name="count">Number of items to return per page.</param>
    /// <param name="page">Page number of results to return.</param>
    /// <param name="cursor">
    /// Parameter for pagination.
    /// Set cursor equal to the <see cref="ResponseMetadata.NextCursor"/> returned by the previous request's <see cref="ReactionItemListResponse.ResponseMetadata"/>.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<ReactionItemListResponse> List(string userId = null, bool full = false, int count = 100, int page = 1, string cursor = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a reaction (emoji) from a file.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Reaction (emoji) name.</param>
    /// <param name="fileId">File to remove reaction from.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveFromFile(string name, string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a reaction (emoji) from a file comment.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Reaction (emoji) name.</param>
    /// <param name="fileCommentId">File comment to remove reaction from.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveFromFileComment(string name, string fileCommentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a reaction (emoji) from a message.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/reactions.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Reaction (emoji) name.</param>
    /// <param name="channelId">Channel where the message to remove reaction from was posted.</param>
    /// <param name="ts">Timestamp of the message to remove reaction from.</param>
    /// <param name="cancellationToken"></param>
    Task RemoveFromMessage(string name, string channelId, string ts, CancellationToken cancellationToken = default);
}

public class ReactionsApi(ISlackApiClient client) : IReactionsApi
{
    public Task AddToMessage(string name, string channelId, string ts, CancellationToken cancellationToken = default) =>
        client.Post("reactions.add", new Args
            {
                { "name", name },
                { "channel", channelId },
                { "timestamp", ts }
            }, cancellationToken);

    public async Task<File> GetForFile(string fileId, bool full = false, CancellationToken cancellationToken = default) =>
        (await client.Get<FileResponse>("reactions.get", new Args
            {
                { "file", fileId },
                { "full", full }
            }, cancellationToken).ConfigureAwait(false))
        .File;

    public async Task<FileComment> GetForFileComment(string fileCommentId, bool full = false, CancellationToken cancellationToken = default) =>
        (await client.Get<FileCommentReactionsResponse>("reactions.get", new Args
            {
                { "file_comment", fileCommentId },
                { "full", full }
            }, cancellationToken).ConfigureAwait(false))
        .Comment;

    public async Task<MessageEvent> GetForMessage(string channelId, string ts, bool full = false, CancellationToken cancellationToken = default) =>
        (await client.Get<MessageReactionsResponse>("reactions.get", new Args
            {
                { "channel", channelId },
                { "timestamp", ts },
                { "full", full }
            }, cancellationToken).ConfigureAwait(false))
        .Message;

    public Task<ReactionItemListResponse> List(string userId = null, bool full = false, int count = 100, int page = 1, string cursor = null, CancellationToken cancellationToken = default) =>
        client.Get<ReactionItemListResponse>("reactions.list", new Args
            {
                { "user", userId },
                { "full", full },
                { "count", count },
                { "page", page },
                { "cursor", cursor }
            }, cancellationToken);

    public Task RemoveFromFile(string name, string fileId, CancellationToken cancellationToken = default) =>
        client.Post("reactions.remove", new Args
            {
                { "name", name },
                { "file", fileId }
            }, cancellationToken);

    public Task RemoveFromFileComment(string name, string fileCommentId, CancellationToken cancellationToken = default) =>
        client.Post("reactions.remove", new Args
            {
                { "name", name },
                { "file_comment", fileCommentId }
            }, cancellationToken);

    public Task RemoveFromMessage(string name, string channelId, string ts, CancellationToken cancellationToken = default) =>
        client.Post("reactions.remove", new Args
            {
                { "name", name },
                { "channel", channelId },
                { "timestamp", ts }
            }, cancellationToken);
}