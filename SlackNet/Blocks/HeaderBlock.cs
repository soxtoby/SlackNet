namespace SlackNet.Blocks;

/// <summary>
/// Displays a larger-sized text block.<br />
/// A header is a plain-text block that displays in a larger, bold font. 
/// Use it to delineate between different groups of content in your app's surfaces.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#header">Slack documentation</a> for more information.</remarks>
[SlackType("header")]
public class HeaderBlock : Block
{
    public HeaderBlock() : base("header") { }

    /// <summary>
    /// The text for the block.
    /// </summary>
    public PlainText Text { get; set; }
}