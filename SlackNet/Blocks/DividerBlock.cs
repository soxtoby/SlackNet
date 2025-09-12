namespace SlackNet.Blocks;

/// <summary>
/// Visually separates pieces of info inside of a message.<br />
/// A content divider, like an <c>&lt;hr&gt;</c>, to split up different blocks inside of a message. 
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/#divider">Slack documentation</a> for more information.</remarks>
[SlackType("divider")]
public class DividerBlock : Block
{
    public DividerBlock() : base("divider") { }
}