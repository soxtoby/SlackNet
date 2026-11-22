using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Approves, denies and lists pending requests to invite people to workspaces in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
/// <remarks>See <a href="https://docs.slack.dev/admins/managing-invite-requests/">managing invite requests</a> for more information.</remarks>
public interface IAdminInviteRequestsApi
{
    /// <summary>
    /// Approve a workspace invite request.
    /// </summary>
    /// <remarks>
    /// <paramref name="teamId"/> is required if your Enterprise organization contains more than one workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.inviteRequests.approve/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="inviteRequestId">
    /// The <see cref="InviteRequest.Id"/> of the request to invite,
    /// from the <see cref="Events.InviteRequested"/> event or <see cref="List"/>.
    /// </param>
    /// <param name="teamId">ID for the workspace where the invite request was made.</param>
    /// <param name="cancellationToken"></param>
    Task Approve(string inviteRequestId, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deny a workspace invite request.
    /// </summary>
    /// <remarks>
    /// <paramref name="teamId"/> is required if your Enterprise organization contains more than one workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.inviteRequests.deny/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="inviteRequestId">
    /// The <see cref="InviteRequest.Id"/> of the request to invite,
    /// from the <see cref="Events.InviteRequested"/> event or <see cref="List"/>.
    /// </param>
    /// <param name="teamId">ID for the workspace where the invite request was made.</param>
    /// <param name="cancellationToken"></param>
    Task Deny(string inviteRequestId, string? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all pending workspace invite requests.
    /// </summary>
    /// <remarks>
    /// <paramref name="teamId"/> is required if your Enterprise organization contains more than one workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.inviteRequests.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">ID for the workspace where the invite requests were made.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="PendingInviteRequestListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The number of results that will be returned by the API on each invocation. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<PendingInviteRequestListResponse> List(
        string? teamId = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminInviteRequestsApi(ISlackApiClient client) : IAdminInviteRequestsApi
{
    public Task Approve(string inviteRequestId, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.inviteRequests.approve", new Args
            {
                { "invite_request_id", inviteRequestId },
                { "team_id", teamId }
            }, cancellationToken);

    public Task Deny(string inviteRequestId, string? teamId = null, CancellationToken cancellationToken = default) =>
        client.Post("admin.inviteRequests.deny", new Args
            {
                { "invite_request_id", inviteRequestId },
                { "team_id", teamId }
            }, cancellationToken);

    public Task<PendingInviteRequestListResponse> List(
        string? teamId = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<PendingInviteRequestListResponse>("admin.inviteRequests.list", new Args
            {
                { "team_id", teamId },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
