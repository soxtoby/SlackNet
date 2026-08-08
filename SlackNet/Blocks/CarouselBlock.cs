using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays related card blocks in a horizontally-scrolling container.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/carousel-block/">Slack documentation</a> for more information.</remarks>
[SlackType("carousel")]
public class CarouselBlock() : Block("carousel")
{
    /// <summary>
    /// A list of cards.
    /// </summary>
    public IList<CardBlock> Elements { get; set; } = [];
}
