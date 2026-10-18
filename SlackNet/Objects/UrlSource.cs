namespace SlackNet;

/// <summary>
/// Displays a clickable URL reference in <see cref="Blocks.TaskCardBlock.Sources"/> or <see cref="WebApi.TaskUpdateChunk.Sources"/>.
/// Cannot be used within other blocks.
/// Slack doesn't validate that the URL resolves.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/block-elements/url-source-element/">Slack documentation</a> for more information.</remarks>
public class UrlSource
{
    /// <summary>The source type sent to Slack. Always <c>url</c>.</summary>
    public string Type { get; } = "url";

    /// <summary>
    /// The URL being referenced.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Display text for the URL.
    /// </summary>
    public string Text { get; set; }
}
