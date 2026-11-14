#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists and cancels requests to install apps in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
/// <remarks>See <a href="https://docs.slack.dev/admins/managing-app-approvals/">managing app approvals</a> for more information.</remarks>
public interface IAdminAppsRequestsApi
{
    /// <summary>
    /// Cancels an app install request that was created within a workspace or an Enterprise organization.
    /// </summary>
    /// <remarks>
    /// <paramref name="enterpriseId"/> is required for cancelling requests which are created in an Enterprise organization.
    /// <paramref name="teamId"/> is required for cancelling requests for other teams on the Enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.requests.cancel/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="requestId">
    /// The ID of the request to cancel.
    /// Can be obtained either directly via the <see cref="Events.AppRequested"/> event, or by <see cref="List"/>.
    /// </param>
    /// <param name="teamId">The ID of the workspace where this request belongs.</param>
    /// <param name="enterpriseId">The ID of the enterprise where this request belongs.</param>
    /// <param name="cancellationToken"></param>
    Task Cancel(string requestId, string? teamId = null, string? enterpriseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists pending app install requests for a workspace.
    /// Only lists requests for installation that have not yet been approved (see <see cref="IAdminAppsApi.Approve"/>)
    /// or restricted (see <see cref="IAdminAppsApi.Restrict"/>).
    /// </summary>
    /// <remarks>
    /// <paramref name="teamId"/> is required if your Enterprise org contains more than one workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.apps.requests.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">The ID of the workspace to list requests for.</param>
    /// <param name="enterpriseId">The ID of the enterprise to list requests for.</param>
    /// <param name="certified">Include requests for certified apps.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AppRequestListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<AppRequestListResponse> List(
        string? teamId = null,
        string? enterpriseId = null,
        bool certified = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsRequestsApi(ISlackApiClient client) : IAdminAppsRequestsApi
{
    public Task Cancel(string requestId, string? teamId = null, string? enterpriseId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.apps.requests.cancel", new Args
            {
                { "request_id", requestId },
                { "team_id", teamId },
                { "enterprise_id", enterpriseId }
            }, cancellationToken);

    public Task<AppRequestListResponse> List(
        string? teamId = null,
        string? enterpriseId = null,
        bool certified = false,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<AppRequestListResponse>("admin.apps.requests.list", new Args
            {
                { "team_id", teamId },
                { "enterprise_id", enterpriseId },
                { "certified", certified },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
