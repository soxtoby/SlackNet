using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface IListDownloadApi
{
    /// <summary>
    /// Initiate a job to export List contents.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.download.start/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List to export.</param>
    /// <param name="includeArchived">Whether to include archived items.</param>
    /// <param name="cancellationToken"></param>
    Task<string> Start(string listId, bool includeArchived = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve List download URL from an export job to download List contents.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/slackLists.download.get/">Slack documentation</a> for more information.</remarks>
    /// <param name="listId">ID of the List to export.</param>
    /// <param name="jobId">The ID of the recently started job to export the List.</param>
    /// <param name="cancellationToken"></param>
    Task<ListDownloadResponse> Get(string listId, string jobId, CancellationToken cancellationToken = default);
    
}

public class ListDownloadApi(ISlackApiClient client) : IListDownloadApi
{
    public async Task<string> Start(string listId, bool includeArchived = false, CancellationToken cancellationToken = default) =>
        (await client.Post<ListDownloadStartResponse>("slackLists.download.start",
            new Args
                {
                    { "list_id", listId },
                    { "include_archived", includeArchived }
                },
            cancellationToken).ConfigureAwait(false))
        .JobId;

    public Task<ListDownloadResponse> Get(string listId, string jobId, CancellationToken cancellationToken = default) =>
        client.Post<ListDownloadResponse>("slackLists.download.get",
            new Args
                {
                    { "list_id", listId },
                    { "job_id", jobId }
                },
            cancellationToken);
}