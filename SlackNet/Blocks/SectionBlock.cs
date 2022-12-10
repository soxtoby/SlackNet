using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A section is one of the most flexible blocks available - it can be used as a simple text block,
/// in combination with text fields, or side-by-side with any of the available block elements.
/// </summary>
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
    public IList<TextObject> Fields { get; set; } = new List<TextObject>();

    /// <summary>
    /// One of the available element objects.
    /// </summary>
    public BlockElement Accessory { get; set; }
}