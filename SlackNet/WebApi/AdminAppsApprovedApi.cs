#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists apps approved for installation in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
/// <remarks>See <a href="https://docs.slack.dev/admins/managing-app-approvals/">managing app approvals</a> for more information.</remarks>
public interface IAdminAppsApprovedApi
{
    /// <summary>
    /// List approved apps for an org or workspace.
    /// </summary>
    /// <remarks>
    /// <paramref name="enterpriseId"/> and <paramref name="teamId"/> cannot be used together.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.approved.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">Return the apps approved for this specific workspace.</param>
    /// <param name="enterpriseId">Return the list of org-wide approved apps.</param>
    /// <param name="certified">Limit the results to only include certified apps. When false, no certified apps will appear in the result.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="ApprovedAppListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<ApprovedAppListResponse> List(
        string? teamId = null,
        string? enterpriseId = null,
        bool certified = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsApprovedApi(ISlackApiClient client) : IAdminAppsApprovedApi
{
    public Task<ApprovedAppListResponse> List(
        string? teamId = null,
        string? enterpriseId = null,
        bool certified = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ApprovedAppListResponse>("admin.apps.approved.list", new Args
            {
                { "team_id", teamId },
                { "enterprise_id", enterpriseId },
                { "certified", certified },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
