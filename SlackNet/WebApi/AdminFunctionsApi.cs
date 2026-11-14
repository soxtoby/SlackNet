#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Looks up the functions of apps in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminFunctionsApi
{
    /// <summary>
    /// Look up functions by a set of apps.
    /// </summary>
    /// <remarks>
    /// Lists all functions installed by a set of app IDs.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.functions.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="appIds">App IDs to get functions for.</param>
    /// <param name="teamId">The team context to retrieve functions from.</param>
    /// <param name="includeNonDistributedFunctions">
    /// Whether to also include functions that are not yet distributed to any users in the function list.
    /// This is needed for admins that are approving an app request and will only work if the team owns the app.
    /// </param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="FunctionListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<FunctionListResponse> List(
        IEnumerable<string> appIds,
        string? teamId = null,
        bool includeNonDistributedFunctions = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminFunctionsApi(ISlackApiClient client) : IAdminFunctionsApi
{
    public Task<FunctionListResponse> List(
        IEnumerable<string> appIds,
        string? teamId = null,
        bool includeNonDistributedFunctions = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<FunctionListResponse>("admin.functions.list", new Args
            {
                { "app_ids", appIds },
                { "team_id", teamId },
                { "include_non_distributed_functions", includeNonDistributedFunctions },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
