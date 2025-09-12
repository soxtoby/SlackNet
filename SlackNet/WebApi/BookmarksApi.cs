using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IBookmarksApi
{
    /// <summary>
    /// Add bookmark to a channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/bookmarks.add/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to add bookmark in.</param>
    /// <param name="title">Title for the bookmark.</param>
    /// <param name="type">Type of the bookmark.</param>
    /// <param name="emoji">Emoji tag to apply to the link.</param>
    /// <param name="entityId">ID of the entity being bookmarked. Only applies to message and file types.</param>
    /// <param name="link">Link to bookmark.</param>
    /// <param name="parentId">Id of this bookmark's parent</param>
    /// <param name="cancellationToken"></param>
    Task<Bookmark> Add(
        string channelId,
        string title,
        BookmarkType type = BookmarkType.Link,
        string emoji = null,
        string entityId = null,
        string link = null,
        string parentId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Edit bookmark.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/bookmarks.edit/">Slack documentation</a> for more information.</remarks>
    /// <param name="bookmarkId">Bookmark to update.</param>
    /// <param name="channelId">Channel to update bookmark in.</param>
    /// <param name="emoji">Emoji tag to apply to the link.</param>
    /// <param name="link">Link to bookmark.</param>
    /// <param name="title">Title for the bookmark.</param>
    /// <param name="cancellationToken"></param>
    Task<Bookmark> Edit(
        string bookmarkId,
        string channelId,
        string emoji = null,
        string link = null,
        string title = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List bookmarks for the channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/bookmarks.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="channelId">Channel to list bookmarks in.</param>
    /// <param name="cancellationToken"></param>
    Task<BookmarkListResponse> List(string channelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove bookmark from the channel.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/bookmarks.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="bookmarkId">Bookmark to remove.</param>
    /// <param name="channelId">Channel to remove bookmark from.</param>
    /// <param name="cancellationToken"></param>
    Task Remove(string bookmarkId, string channelId, CancellationToken cancellationToken = default);
}

public class BookmarksApi(ISlackApiClient client) : IBookmarksApi
{
    public async Task<Bookmark> Add(
        string channelId,
        string title,
        BookmarkType type = BookmarkType.Link,
        string emoji = null,
        string entityId = null,
        string link = null,
        string parentId = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<BookmarkResponse>("bookmarks.add", new Args
            {
                { "channel_id", channelId },
                { "title", title },
                { "type", type },
                { "emoji", emoji },
                { "entity_id", entityId },
                { "link", link },
                { "parent_id", parentId }
            }, cancellationToken).ConfigureAwait(false))
        .Bookmark;

    public async Task<Bookmark> Edit(
        string bookmarkId,
        string channelId,
        string emoji = null,
        string link = null,
        string title = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<BookmarkResponse>("bookmarks.edit", new Args
            {
                { "bookmark_id", bookmarkId },
                { "channel_id", channelId },
                { "emoji", emoji },
                { "link", link },
                { "title", title }
            }, cancellationToken).ConfigureAwait(false))
        .Bookmark;

    public Task<BookmarkListResponse> List(string channelId, CancellationToken cancellationToken = default) =>
        client.Post<BookmarkListResponse>("bookmarks.list", new Args { { "channel_id", channelId } }, cancellationToken);

    public Task Remove(string bookmarkId, string channelId, CancellationToken cancellationToken = default) =>
        client.Post("bookmarks.remove", new Args
            {
                { "bookmark_id", bookmarkId },
                { "channel_id", channelId }
            }, cancellationToken);
}