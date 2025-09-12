using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays contextual info, which can include both images and text.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/#context">Slack documentation</a> for more information.</remarks>
[SlackType("context")]
public class ContextBlock : Block
{
    public ContextBlock() : base("context") { }

    /// <summary>
    /// An array of image elements and text objects. 
    /// </summary>
    public IList<IContextElement> Elements { get; set; } = [];
}