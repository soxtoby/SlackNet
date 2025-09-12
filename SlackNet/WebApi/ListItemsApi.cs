#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IListItemsApi
{
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
}

public class ListItemsApi(ISlackApiClient client) : IListItemsApi
{
    public Task Delete(string listId, string itemId, CancellationToken cancellationToken = default) =>
        client.Post("slackLists.items.delete", new Args
            {
                { "list_id", listId }, 
                { "id", itemId }
            }, cancellationToken);
    
    public Task DeleteMultiple(string listId, IEnumerable<string> itemIds, CancellationToken cancellationToken = default) =>
        client.Post("slackLists.items.deleteMultiple", new Args
            {
                { "list_id", listId },
                { "ids", itemIds }
            }, cancellationToken);
    
    public Task<ListItemsResponse> List(string listId, string? cursor = null, bool archived = false, CancellationToken cancellationToken = default) =>
        client.Post<ListItemsResponse>("slackLists.items.list", new Args
            {
                { "list_id", listId },
                { "cursor", cursor },
                { "archived", archived }
            }, cancellationToken);
}