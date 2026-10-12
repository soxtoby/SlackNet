namespace SlackNet.Blocks;

/// <summary>
/// Displays a clickable URL reference. This element can only be used in a <see cref="TaskCardBlock"/>.
/// Slack doesn't validate that the URL resolves.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/block-elements/url-source-element/">Slack documentation</a> for more information.</remarks>
[SlackType("url")]
public class UrlSourceElement() : BlockElement("url")
{
    /// <summary>
    /// The URL being referenced.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Display text for the URL.
    /// </summary>
    public string Text { get; set; }
}