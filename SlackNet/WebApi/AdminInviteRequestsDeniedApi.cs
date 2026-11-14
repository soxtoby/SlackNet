#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists requests to invite people to workspaces in an Enterprise organization that have been denied.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
/// <remarks>See <a href="https://docs.slack.dev/admins/managing-invite-requests/">managing invite requests</a> for more information.</remarks>
public interface IAdminInviteRequestsDeniedApi
{
    /// <summary>
    /// List all denied workspace invite requests.
    /// </summary>
    /// <remarks>
    /// <paramref name="teamId"/> is required if your Enterprise organization contains more than one workspace.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.inviteRequests.denied.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamId">ID for the workspace where the invite requests were made.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="DeniedInviteRequestListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The number of results that will be returned by the API on each invocation. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<DeniedInviteRequestListResponse> List(
        string? teamId = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public class AdminInviteRequestsDeniedApi(ISlackApiClient client) : IAdminInviteRequestsDeniedApi
{
    public Task<DeniedInviteRequestListResponse> List(
        string? teamId = null,
        string? cursor = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<DeniedInviteRequestListResponse>("admin.inviteRequests.denied.list", new Args
            {
                { "team_id", teamId },
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
