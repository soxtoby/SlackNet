#nullable enable
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Lists the third-party app MCP servers approved for an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAppsMcpServersApi
{
    /// <summary>
    /// List third-party app MCP servers approved for an organization, derived from the org's MCP server allowlist.
    /// Entries reflect allowlist state, not install or scope liveness: servers of apps that are uninstalled (but not deleted) are still listed.
    /// To manage who can use a given MCP server, use <see cref="IAdminAppsMcpServersPermissionsApi"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.mcp.servers.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="cursor">
    /// Paginate through collections of data by setting the cursor parameter to a <see cref="ResponseMetadata.NextCursor"/> property
    /// returned by a previous request's <see cref="AppMcpServerListResponse.ResponseMetadata"/>.
    /// Default value fetches the first "page" of the collection.
    /// </param>
    /// <param name="limit">The maximum number of items to return. Defaults to 100.</param>
    /// <param name="cancellationToken"></param>
    Task<AppMcpServerListResponse> List(string? cursor = null, int? limit = null, CancellationToken cancellationToken = default);
}

public class AdminAppsMcpServersApi(ISlackApiClient client) : IAdminAppsMcpServersApi
{
    public Task<AppMcpServerListResponse> List(string? cursor = null, int? limit = null, CancellationToken cancellationToken = default) =>
        client.Post<AppMcpServerListResponse>("admin.apps.mcp.servers.list", new Args
            {
                { "cursor", cursor },
                { "limit", limit }
            }, cancellationToken);
}
