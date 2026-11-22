using System.Collections.Generic;

namespace SlackNet.AppManifests;

/// <summary>
/// An <a href="https://docs.slack.dev/ai/slack-mcp-server/developing/">MCP server</a> used by the app.
/// </summary>
public class McpServer
{
    /// <summary>
    /// The key of an external auth provider in <see cref="AppManifest.ExternalAuthProviders"/> to use for authentication.
    /// </summary>
    public string? AuthProviderKey { get; set; }

    /// <summary>
    /// The authentication type.
    /// </summary>
    public McpAuthType? AuthType { get; set; }

    /// <summary>
    /// A short description of what the MCP server lets Slackbot do.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// HTTP headers to send with requests to the MCP server.
    /// </summary>
    public IDictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// An emoji to display as the icon for the MCP server, such as <c>:robot_face:</c>.
    /// </summary>
    public string? IconEmoji { get; set; }

    /// <summary>
    /// The URL of the MCP server.
    /// </summary>
    public required string Url { get; set; }
}
