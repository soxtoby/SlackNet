#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

public interface IExternalTeamsApi
{
    /// <summary>
    /// Disconnect an external organization.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/team.externalTeams.disconnect/">Slack documentation</a> for more information.</remarks>
    /// <param name="targetTeamId">The team ID of the target team.</param>
    /// <param name="cancellationToken"></param>
    Task Disconnect(string targetTeamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a list of all the external teams connected and details about the connection.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/team.externalTeams.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="limit">The maximum number of items to return per page.</param>
    /// <param name="cursor">Paginate through collections of data by setting parameter to the <see cref="ResponseMetadata.NextCursor"/> returned by a previous request's <see cref="ExternalTeamsListResponse.ResponseMetadata"/>. If not provided, the first page of the collection is returned.</param>
    /// <param name="sortField">Name of the parameter that we are sorting by (snake-cased name of property in <see cref="ExternalTeam"/>).</param>
    /// <param name="sortDirection">Direction to sort in.</param>
    /// <param name="slackConnectPrefFilter">Filters connected orgs by Slack Connect pref override(s).</param>
    /// <param name="workspaceIds">Shows connected orgs which are connected on a specified encoded workspace ID.</param>
    /// <param name="connectionStatusFilter">Status of the connected team.</param>
    /// <param name="cancellationToken"></param>
    Task<ExternalTeamsListResponse> List(
        int limit = 20,
        string? cursor = null,
        string? sortField = "team_name",
        SortDirection sortDirection = SortDirection.Ascending,
        IEnumerable<SlackConnectPref>? slackConnectPrefFilter = null,
        IEnumerable<string>? workspaceIds = null,
        string? connectionStatusFilter = null,
        CancellationToken cancellationToken = default);
}

public class ExternalTeamsApi(ISlackApiClient client) : IExternalTeamsApi
{
    public Task Disconnect(string targetTeamId, CancellationToken cancellationToken = default) =>
        client.Post("team.externalTeams.disconnect", new Args { { "target_team_id", targetTeamId } }, cancellationToken);
    
    public Task<ExternalTeamsListResponse> List(
        int limit = 20,
        string? cursor = null,
        string? sortField = "team_name",
        SortDirection sortDirection = SortDirection.Ascending,
        IEnumerable<SlackConnectPref>? slackConnectPrefFilter = null,
        IEnumerable<string>? workspaceIds = null,
        string? connectionStatusFilter = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Get<ExternalTeamsListResponse>("team.externalTeams.list", new Args
            {
                { "limit", limit },
                { "cursor", cursor },
                { "sort_field", sortField },
                { "sort_direction", sortDirection },
                { "slack_connect_pref_filter", slackConnectPrefFilter },
                { "workspace_filter", workspaceIds },
                { "connection_status_filter", connectionStatusFilter }
            }, cancellationToken);
}