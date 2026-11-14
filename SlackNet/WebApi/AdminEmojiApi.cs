#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages custom emoji across an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminEmojiApi
{
    /// <summary>
    /// Add an emoji.
    /// </summary>
    /// <remarks>
    /// This method adds a custom emoji across an Enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.emoji.add/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="emojiName">
    /// The name of the emoji to be added (using lower-case letters only).
    /// Colons (<c>:myemoji:</c>) around the value are not required, although they may be included.
    /// </param>
    /// <param name="url">
    /// The URL of a file to use as an image for the emoji.
    /// Square images with transparent backgrounds work best.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task Add(string emojiName, string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add an emoji alias.
    /// </summary>
    /// <remarks>
    /// This method adds an alias for an emoji across an Enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.emoji.addAlias/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="aliasName">The new alias for the specified emoji. Any wrapping whitespace or colons will be automatically trimmed.</param>
    /// <param name="emojiName">Name of the emoji for which the alias is being made. Any wrapping whitespace or colons will be automatically trimmed.</param>
    /// <param name="cancellationToken"></param>
    Task AddAlias(string aliasName, string emojiName, CancellationToken cancellationToken = default);

    /// <summary>
    /// List emoji for an Enterprise organization.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.emoji.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="CustomEmojiListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<CustomEmojiListResponse> List(string? cursor = null, int? limit = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove an emoji across an Enterprise organization.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.emoji.remove/">Slack documentation</a> for more information.</remarks>
    /// <param name="emojiName">
    /// The name of the emoji to be removed.
    /// Colons (<c>:myemoji:</c>) around the value are not required, although they may be included.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task Remove(string emojiName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rename an emoji.
    /// </summary>
    /// <remarks>
    /// This method renames a custom emoji across an Enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.emoji.rename/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="emojiName">
    /// The name of the emoji to be renamed.
    /// Colons (<c>:myemoji:</c>) around the value are not required, although they may be included.
    /// </param>
    /// <param name="newEmojiName">The new name of the emoji.</param>
    /// <param name="cancellationToken"></param>
    Task Rename(string emojiName, string newEmojiName, CancellationToken cancellationToken = default);
}

public class AdminEmojiApi(ISlackApiClient client) : IAdminEmojiApi
{
    public Task Add(string emojiName, string url, CancellationToken cancellationToken = default) =>
        client.Get("admin.emoji.add", new Args
            {
                { "name", emojiName },
                { "url", url }
            }, cancellationToken);

    public Task AddAlias(string aliasName, string emojiName, CancellationToken cancellationToken = default) =>
        client.Get("admin.emoji.addAlias", new Args
            {
                { "name", aliasName },
                { "alias_for", emojiName }
            }, cancellationToken);

    public Task<CustomEmojiListResponse> List(string? cursor = null, int? limit = null, CancellationToken cancellationToken = default) =>
        client.Get<CustomEmojiListResponse>("admin.emoji.list", new Args
            {
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);

    public Task Remove(string emojiName, CancellationToken cancellationToken = default) =>
        client.Get("admin.emoji.remove", new Args { { "name", emojiName } }, cancellationToken);

    public Task Rename(string emojiName, string newEmojiName, CancellationToken cancellationToken = default) =>
        client.Get("admin.emoji.rename", new Args
            {
                { "name", emojiName },
                { "new_name", newEmojiName }
            }, cancellationToken);
}
