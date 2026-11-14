#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists the owners of workspaces in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminTeamsOwnersApi
{
    /// <summary>
    /// List all of the owners on a given workspace.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.teams.owners.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="teamId">The workspace to list the owners of.</param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="TeamOwnerListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<TeamOwnerListResponse> List(string teamId, int? limit = null, string? cursor = null, CancellationToken cancellationToken = default);
}

public class AdminTeamsOwnersApi(ISlackApiClient client) : IAdminTeamsOwnersApi
{
    public Task<TeamOwnerListResponse> List(string teamId, int? limit = null, string? cursor = null, CancellationToken cancellationToken = default) =>
        client.Get<TeamOwnerListResponse>("admin.teams.owners.list", new Args
            {
                { "team_id", teamId },
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
}
