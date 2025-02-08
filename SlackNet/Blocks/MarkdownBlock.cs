namespace SlackNet.Blocks;

/// <summary>
/// Displays formatted markdown.
/// This block can be used with AI apps when you expect a markdown response from an LLM that can get lost in translation rendering in Slack.
/// Providing it in a markdown block leaves the translating to Slack to ensure your message appears as intended.
/// Note that passing a single block may result in multiple blocks after translation.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#markdown">Slack documentation</a> for more information.</remarks>
[SlackType("markdown")]
public class MarkdownBlock() : Block("markdown")
{
    /// <summary>
    /// The standard markdown-formatted text.
    /// </summary>
    public string Text { get; set; }
}