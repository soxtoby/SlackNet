using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet.Blocks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IListApi
{
    /// <summary>
    /// Create a List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.create/">Slack documentation</a> for more information.</remarks>
    /// <param name="name">Name of the List.</param>
    /// <param name="descriptionBlocks">A rich text description of the List.</param>
    /// <param name="schema">Column definition for the List.</param>
    /// <param name="copyFromListId">ID of the List to copy.</param>
    /// <param name="includeCopiedListRecords">Boolean indicating whether to include records when a List is copied.</param>
    /// <param name="todoMode">Boolean indicating whether the List should be used to track todo tasks.</param>
    /// <param name="cancellationToken"></param>
    Task<SlackListResponse> Create(
        string name,
        IEnumerable<RichTextBlock> descriptionBlocks = null,
        IEnumerable<ColumnSchema> schema = null,
        string copyFromListId = null,
        bool? includeCopiedListRecords = null,
        bool? todoMode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a List.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.update/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">The ID of the List to update.</param>
    /// <param name="name">The updated name of the List.</param>
    /// <param name="descriptionBlocks">A rich text description of the List.</param>
    /// <param name="todoMode">Boolean indicating whether the List should be in todo mode.</param>
    /// <param name="cancellationToken"></param>
    Task<SlackListResponse> Update(
        string listId,
        string name = null,
        IEnumerable<RichTextBlock> descriptionBlocks = null,
        bool? todoMode = null,
        CancellationToken cancellationToken = default);
}

public class ListApi(ISlackApiClient client) : IListApi
{
    public Task<SlackListResponse> Create(
        string name,
        IEnumerable<RichTextBlock> descriptionBlocks = null,
        IEnumerable<ColumnSchema> schema = null,
        string copyFromListId = null,
        bool? includeCopiedListRecords = null,
        bool? todoMode = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<SlackListResponse>("slackLists.create",
            new Args
                {
                    { "name", name },
                    { "description_blocks", descriptionBlocks },
                    { "schema", schema },
                    { "copy_from_list_id", copyFromListId },
                    { "include_copied_list_records", includeCopiedListRecords },
                    { "todo_mode", todoMode }
                },
            cancellationToken);

    public Task<SlackListResponse> Update(
        string listId,
        string name = null,
        IEnumerable<RichTextBlock> descriptionBlocks = null,
        bool? todoMode = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<SlackListResponse>("slackLists.update",
            new Args
                {
                    { "id", listId },
                    { "name", name },
                    { "description_blocks", descriptionBlocks },
                    { "todo_mode", todoMode }
                },
            cancellationToken);
}