namespace SlackNet.Blocks;

/// <summary>
/// A content divider, like an <c>&lt;hr&gt;</c>, to split up different blocks inside of a message.
/// </summary>
[SlackType("divider")]
public class DividerBlock : Block
{
    public DividerBlock() : base("divider") { }
}