using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A block that is used to hold interactive elements.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/#actions">Slack documentation</a> for more information.</remarks>
[SlackType("actions")]
public class ActionsBlock : Block
{
    public ActionsBlock() : base("actions") { }

    /// <summary>
    /// An array of interactive element objects.
    /// </summary>
    public IList<IActionElement> Elements { get; set; } = [];
}