#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// An app's MCP server, with the access control permission for who can use it.
/// </summary>
public class McpServerPermissions
{
    /// <summary>
    /// Encoded ID of the MCP server.
    /// </summary>
    public required string ServerId { get; set; }

    /// <summary>
    /// Name of the MCP server.
    /// </summary>
    public required string ServerName { get; set; }

    /// <summary>
    /// The URL of the MCP server.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// The type of permission that defines who can use this MCP server.
    /// </summary>
    public AppPermissionType PermissionType { get; set; }

    /// <summary>
    /// The users that the <see cref="PermissionType"/> applies to,
    /// for <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>.
    /// </summary>
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// The usergroups that the <see cref="PermissionType"/> applies to,
    /// for <see cref="AppPermissionType.NamedEntities"/> or <see cref="AppPermissionType.NamedEntitiesExclude"/>.
    /// </summary>
    public IList<string> UsergroupIds { get; set; } = [];

    /// <summary>
    /// Whether the server's permission is inherited from the app's permissions (see <see cref="AppPermissions"/>), rather than set on the server itself.
    /// </summary>
    public bool IsInherited { get; set; }
}
