#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Creates and lists workspaces in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminTeamsApi
{
    /// <summary>
    /// Create an Enterprise team.
    /// </summary>
    /// <remarks>
    /// This Admin API creates a workspace for an enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.teams.create/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="teamDomain">Team domain (for example, slacksoftballteam).</param>
    /// <param name="teamName">Team name (for example, Slack Softball Team).</param>
    /// <param name="teamDescription">Description for the team.</param>
    /// <param name="teamDiscoverability">Who can join the team.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The ID of the created team.</returns>
    Task<string> Create(
        string teamDomain,
        string teamName,
        string? teamDescription = null,
        TeamDiscoverability? teamDiscoverability = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List all teams in an Enterprise organization.
    /// </summary>
    /// <remarks>
    /// This Admin API lists workspaces in an enterprise organization.
    /// See the <a href="https://docs.slack.dev/reference/methods/admin.teams.list/">Slack documentation</a> for more information.
    /// </remarks>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="EnterpriseTeamListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="cancellationToken"></param>
    Task<EnterpriseTeamListResponse> List(int? limit = null, string? cursor = null, CancellationToken cancellationToken = default);
}

public class AdminTeamsApi(ISlackApiClient client) : IAdminTeamsApi
{
    public async Task<string> Create(
        string teamDomain,
        string teamName,
        string? teamDescription = null,
        TeamDiscoverability? teamDiscoverability = null,
        CancellationToken cancellationToken = default
    ) =>
        (await client.Post<TeamCreateResponse>("admin.teams.create", new Args
            {
                { "team_domain", teamDomain },
                { "team_name", teamName },
                { "team_description", teamDescription },
                { "team_discoverability", teamDiscoverability }
            }, cancellationToken).ConfigureAwait(false))
        .Team;

    public Task<EnterpriseTeamListResponse> List(int? limit = null, string? cursor = null, CancellationToken cancellationToken = default) =>
        client.Post<EnterpriseTeamListResponse>("admin.teams.list", new Args
            {
                { "limit", limit },
                { "cursor", cursor }
            }, cancellationToken);
}
