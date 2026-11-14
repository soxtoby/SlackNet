#nullable enable
namespace SlackNet;

/// <summary>
/// A third-party app MCP server approved for an organization.
/// </summary>
public class AppMcpServer
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
    /// The authentication type for the MCP server.
    /// Null if Slack returned a type SlackNet doesn't know.
    /// </summary>
    public McpAuthType? AuthType { get; set; }

    /// <summary>
    /// Encoded ID of the app that provides this MCP server.
    /// </summary>
    public required string AppId { get; set; }

    /// <summary>
    /// Name of the app that provides this MCP server.
    /// </summary>
    public required string AppName { get; set; }
}
