namespace SlackNet;

/// <summary>
/// An app the skill needs.
/// App servers use <see cref="AppId"/> and <see cref="ServerId"/>, Salesforce servers use <see cref="PermissionNamespace"/>,
/// and built-in servers use <see cref="ServiceType"/> alone.
/// </summary>
public class McpServer
{
    /// <summary>
    /// The server's service type: <c>app</c> for an app's MCP server, <c>salesforce</c>, or a built-in service type.
    /// </summary>
    public required string ServiceType { get; set; }

    /// <summary>
    /// App ID. Present for app servers (<see cref="ServiceType"/> is <c>app</c>).
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// ID of the app's MCP server. Present for app servers (<see cref="ServiceType"/> is <c>app</c>).
    /// </summary>
    public string? ServerId { get; set; }

    /// <summary>
    /// Permission namespace. Present for Salesforce servers (<see cref="ServiceType"/> is <c>salesforce</c>).
    /// </summary>
    public string? PermissionNamespace { get; set; }

    /// <summary>
    /// Copies only the properties that identify the server, so that properties added by derived types,
    /// like <see cref="SkillMcpServer.DisplayName"/>, aren't sent in requests.
    /// </summary>
    internal McpServer AsReference() => new()
        {
            ServiceType = ServiceType,
            AppId = AppId,
            ServerId = ServerId,
            PermissionNamespace = PermissionNamespace
        };
}

/// <summary>
/// An app or MCP server a skill declares it needs.
/// Listing a server does not grant access to it; normal OAuth and server permissions still apply when the skill runs.
/// </summary>
public class SkillMcpServer : McpServer
{
    /// <summary>
    /// The server's current name as the caller sees it, or null when the server isn't available to the caller.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Whether the caller can use this server.
    /// </summary>
    public bool IsAvailable { get; set; }
}
