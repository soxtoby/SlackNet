﻿using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays text, possibly alongside block elements.<br />
/// A section can be used as a text block, in combination with text fields, or side-by-side with certain block elements.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/#section">Slack documentation</a> for more information.</remarks>
[SlackType("section")]
public class SectionBlock : Block
{
    public SectionBlock() : base("section") { }

    /// <summary>
    /// The text for the block.
    /// </summary>
    public TextObject Text { get; set; }

    /// <summary>
    /// An array of text objects. Any text objects included with fields will be rendered in a compact format that allows for 2 columns of side-by-side text.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<TextObject> Fields { get; set; } = [];

    /// <summary>
    /// One of the available element objects.
    /// </summary>
    public BlockElement Accessory { get; set; }

    /// <summary>
    /// Whether or not this section block's text should always expand when rendered.
    /// If false or not provided, it may be rendered with a 'see more' option to expand and show the full text.
    /// For AI Assistant apps, this allows the app to post long messages without users needing to click 'see more' to expand the message.
    /// </summary>
    public bool Expand { get; set; }
}