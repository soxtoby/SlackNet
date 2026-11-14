#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Args = System.Collections.Generic.Dictionary<string, object?>;

namespace SlackNet.WebApi;

/// <summary>
/// Manages who can use an app's MCP servers in an Enterprise organization.
/// Admin API methods require a user token from an app installed on the entire Enterprise organization, rather than on a workspace,
/// which you can use with <see cref="ISlackApiClient.WithAccessToken"/>.
/// </summary>
public interface IAdminAppsMcpServersPermissionsApi
{
    /// <summary>
    /// List MCP servers for an app with their access control permissions.
    /// To list the MCP servers approved across the org, use <see cref="IAdminAppsMcpServersApi.List"/>.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.mcp.servers.permissions.list/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="cancellationToken"></param>
    Task<IList<McpServerPermissions>> List(string appId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the access control permission for who can use an MCP server.
    /// When the <paramref name="permissionType"/> is <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>,
    /// pass either or both <paramref name="userIds"/> and <paramref name="usergroupIds"/> to define the entities the rule applies to.
    /// </summary>
    /// <remarks>See the <a href="https://docs.slack.dev/reference/methods/admin.apps.mcp.servers.permissions.set/">Slack documentation</a> for more information.</remarks>
    /// <param name="appId">Encoded ID of the app.</param>
    /// <param name="serverId">Encoded ID of the MCP server.</param>
    /// <param name="permissionType">The type of permission that defines who can use this MCP server.</param>
    /// <param name="userIds">List of user IDs to set for <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>.</param>
    /// <param name="usergroupIds">List of encoded usergroup IDs.</param>
    /// <param name="cancellationToken"></param>
    Task<McpServerPermissions> Set(
        string appId,
        string serverId,
        AppPermissionType permissionType,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        CancellationToken cancellationToken = default);
}

public class AdminAppsMcpServersPermissionsApi(ISlackApiClient client) : IAdminAppsMcpServersPermissionsApi
{
    public async Task<IList<McpServerPermissions>> List(string appId, CancellationToken cancellationToken = default) =>
        (await client.Post<McpServerPermissionsListResponse>("admin.apps.mcp.servers.permissions.list", new Args
            {
                { "app_id", appId }
            }, cancellationToken).ConfigureAwait(false)).Servers;

    public Task<McpServerPermissions> Set(
        string appId,
        string serverId,
        AppPermissionType permissionType,
        IEnumerable<string>? userIds = null,
        IEnumerable<string>? usergroupIds = null,
        CancellationToken cancellationToken = default
    ) =>
        client.Post<McpServerPermissions>("admin.apps.mcp.servers.permissions.set", new Args
            {
                { "app_id", appId },
                { "server_id", serverId },
                { "permission_type", permissionType },
                { "user_ids", userIds },
                { "usergroup_ids", usergroupIds }
            }, cancellationToken);
}
