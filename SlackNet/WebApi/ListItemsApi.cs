#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IListItemsApi
{
    /// <summary>
    /// Add a new item to an existing List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.items.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List to add the item to.</param>
    /// <param name="duplicatedItemId">ID of the record to make a copy of.</param>
    /// <param name="parentItemId">ID of the parent record for this subtask.</param>
    /// <param name="initialFields">Initial item data.</param>
    /// <param name="cancellationToken"></param>
    Task<ListItem> Create(
        string listId,
        string? duplicatedItemId = null,
        string? parentItemId = null,
        IEnumerable<ListItemField>? initialFields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates cells in a List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.items.update/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List to add or update cells.</param>
    /// <param name="cells">Cells to update.</param>
    /// <param name="cancellationToken"></param>
    Task Update(string listId, IEnumerable<ListItemFieldUpdate> cells, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an item from an existing List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.items.delete/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List containing the item.</param>
    /// <param name="itemId">ID of item to delete.</param>
    /// <param name="cancellationToken"></param>
    Task Delete(string listId, string itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple items from an existing List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.items.deleteMultiple/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List containing the items.</param>
    /// <param name="itemIds">IDs of items to delete.</param>
    /// <param name="cancellationToken"></param>
    Task DeleteMultiple(string listId, IEnumerable<string> itemIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get records from a List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.items.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List.</param>
    /// <param name="cursor">Next cursor for pagination.</param>
    /// <param name="archived">Boolean indicating whether archived items or normal items should be returned.</param>
    /// <param name="cancellationToken"></param>
    Task<ListItemsResponse> List(string listId, string? cursor = null, bool archived = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a row from a List.
    /// </summary>
    /// <param name="listId">ID of the List.</param>
    /// <param name="rowId">ID of the row to get.</param>
    /// <param name="includeIsSubscribed">Set to True to include <see cref="ListItem.IsSubscribed"/> data for the returned List row.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ListItemInfo> Info(string listId, string rowId, bool includeIsSubscribed = false, CancellationToken cancellationToken = default);
}

public class ListItemsApi(ISlackApiClient client) : IListItemsApi
{
    public async Task<ListItem> Create(
        string listId,
        string? duplicatedItemId = null,
        string? parentItemId = null,
        IEnumerable<ListItemField>? initialFields = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<ListItemResponse>("slackLists.items.create",
            new Args
                {
                    { "list_id", listId },
                    { "initial_fields", initialFields },
                    { "duplicated_item_id", duplicatedItemId },
                    { "parent_item_id", parentItemId }
                },
            cancellationToken).ConfigureAwait(false))
        .Item;

    public Task Update(string listId, IEnumerable<ListItemFieldUpdate> cells, CancellationToken cancellationToken = default) =>
        client.Post("slackLists.items.update",
            new Args
                {
                    { "list_id", listId },
                    { "cells", cells }
                },
            cancellationToken);

    public Task Delete(string listId, string itemId, CancellationToken cancellationToken = default) =>
        client.Post("slackLists.items.delete",
            new Args
                {
                    { "list_id", listId },
                    { "id", itemId }
                },
            cancellationToken);

    public Task DeleteMultiple(string listId, IEnumerable<string> itemIds, CancellationToken cancellationToken = default) =>
        client.Post("slackLists.items.deleteMultiple",
            new Args
                {
                    { "list_id", listId },
                    { "ids", itemIds }
                },
            cancellationToken);

    public Task<ListItemsResponse> List(string listId, string? cursor = null, bool archived = false, CancellationToken cancellationToken = default) =>
        client.Post<ListItemsResponse>("slackLists.items.list",
            new Args
                {
                    { "list_id", listId },
                    { "cursor", cursor },
                    { "archived", archived }
                },
            cancellationToken);

    public Task<ListItemInfo> Info(string listId, string rowId, bool includeIsSubscribed = false, CancellationToken cancellationToken = default) =>
        client.Post<ListItemInfo>("slackLists.items.info",
            new Args
                {
                    { "list_id", listId },
                    { "id", rowId },
                    { "include_is_subscribed", includeIsSubscribed }
                },
            cancellationToken);
}