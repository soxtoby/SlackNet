namespace SlackNet.WebApi;

/// <summary>
/// Format in which to return canvas content.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/methods/canvases.getContent/">Slack documentation</a> for more information.</remarks>
public enum CanvasContentType
{
    /// <summary>
    /// The canvas as markdown, in the same format that <see cref="ICanvasesApi.Create"/> and <see cref="ICanvasesApi.Edit"/> accept.
    /// </summary>
    Markdown,

    /// <summary>
    /// The canvas as HTML.
    /// </summary>
    Html
}
