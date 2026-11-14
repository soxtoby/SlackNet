#nullable enable
namespace SlackNet;

/// <summary>
/// Details of the external resource an agent session is centered on.
/// Only the properties that are set are sent, so leave a property <c>null</c> to keep its current value.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/ai/slack-code/code-channel-properties/#agent-resource">Slack documentation</a> for more information.</remarks>
public class AgentResource
{
    /// <summary>
    /// URL of the external resource.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Type of the external resource.
    /// </summary>
    public string? ResourceType { get; set; }

    /// <summary>
    /// Display title of the external resource.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Provider of the external resource.
    /// </summary>
    public string? Provider { get; set; }
}
