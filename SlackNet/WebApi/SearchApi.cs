using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace SlackNet.WebApi;

public interface ISearchApi
{
    /// <summary>
    /// Allows users and applications to search both messages and files in a single call.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/search.all/">Slack documentation</a> for more information.</remarks>
    /// <param name="query">Search query. May contains booleans, etc.</param>
    /// <param name="sort">Return matches sorted by either score or timestamp.</param>
    /// <param name="sortDirection">Change sort direction to ascending or descending</param>
    /// <param name="highlight">Pass a value of True to enable query highlight markers.</param>
    /// <param name="count">Number of items to return per page.</param>
    /// <param name="page">Page number of results to return.</param>
    /// <param name="cancellationToken"></param>
    Task<SearchResponse> All(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns files matching a search query.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/search.files/">Slack documentation</a> for more information.</remarks>
    /// <param name="query">Search query. May contains booleans, etc.</param>
    /// <param name="sort">Return matches sorted by either score or timestamp.</param>
    /// <param name="sortDirection">Change sort direction to ascending or descending</param>
    /// <param name="highlight">Pass a value of True to enable query highlight markers.</param>
    /// <param name="count">Number of items to return per page.</param>
    /// <param name="page">Page number of results to return.</param>
    /// <param name="cancellationToken"></param>
    Task<FileSearchResponse> Files(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns messages matching a search query.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/search.messages/">Slack documentation</a> for more information.</remarks>
    /// <param name="query">Search query. May contains booleans, etc.</param>
    /// <param name="sort">Return matches sorted by either score or timestamp.</param>
    /// <param name="sortDirection">Change sort direction to ascending or descending</param>
    /// <param name="highlight">Pass a value of True to enable query highlight markers.</param>
    /// <param name="count">Number of items to return per page.</param>
    /// <param name="page">Page number of results to return.</param>
    /// <param name="cancellationToken"></param>
    Task<MessageSearchResponse> Messages(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    );
}

public class SearchApi(ISlackApiClient client) : ISearchApi
{
    public Task<SearchResponse> All(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<SearchResponse>("search.all", new Args
                {
                    { "query", query },
                    { "sort", sort },
                    { "sort_dir", sortDirection },
                    { "highlight", highlight },
                    { "count", count },
                    { "page", page }
                },
            cancellationToken);

    public Task<FileSearchResponse> Files(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<FileSearchResponse>("search.files", new Args
                {
                    { "query", query },
                    { "sort", sort },
                    { "sort_dir", sortDirection },
                    { "highlight", highlight },
                    { "count", count },
                    { "page", page }
                },
            cancellationToken);

    public Task<MessageSearchResponse> Messages(
        string query,
        SortBy sort = SortBy.Score,
        SortDirection sortDirection = SortDirection.Descending,
        bool highlight = false,
        int count = 20,
        int page = 1,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<MessageSearchResponse>("search.messages", new Args
                {
                    { "query", query },
                    { "sort", sort },
                    { "sort_dir", sortDirection },
                    { "highlight", highlight },
                    { "count", count },
                    { "page", page }
                },
            cancellationToken);
}