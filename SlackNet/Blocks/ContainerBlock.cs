using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// A general-purpose wrapper for grouping child blocks together, with a configurable size.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/blocks/container-block/">Slack documentation</a> for more information.</remarks>
[SlackType("container")]
public class ContainerBlock() : Block("container")
{
    /// <summary>
    /// List of included blocks.
    /// </summary>
    public IList<Block> ChildBlocks { get; set; } = [];

    /// <summary>
    /// Title of the container, using plain-text formatting.
    /// Either <see cref="Title"/> or <see cref="RichTextTitle"/> is required.
    /// </summary>
    public PlainText Title { get; set; }

    /// <summary>
    /// The title for the container as a rich text block. If both <see cref="Title"/> and <see cref="RichTextTitle"/> are provided,
    /// <see cref="RichTextTitle"/> takes precedence.
    /// </summary>
    public RichTextBlock RichTextTitle { get; set; }

    /// <summary>
    /// Subtitle of the container.
    /// </summary>
    public TextObject Subtitle { get; set; }

    /// <summary>
    /// Sets the width of the container block. The narrow, standard, and wide options use a platform-determined constrained width.
    /// The full option expands to fill the available space. The default is standard.
    /// </summary>
    [IgnoreIfDefault]
    public ContainerWidth Width { get; set; }

    /// <summary>
    /// Link to the small image used next to the card's title and subtitle.
    /// </summary>
    public Image Icon { get; set; }

    /// <summary>
    /// When true, the block can be collapsed to show only the title.
    /// </summary>
    public bool IsCollapsible { get; set; }

    /// <summary>
    /// When true and <see cref="IsCollapsible"/> is also true, the block initially renders in a collapsed state.
    /// </summary>
    public bool DefaultCollapsed { get; set; }

    /// <summary>
    /// When true, a visible border is rendered below the header to visually separate it from the content.
    /// Only applies when the block is not collapsible.
    /// </summary>
    public bool HasHeaderDivider { get; set; }
}

/// <summary>Controls how much horizontal space a <see cref="ContainerBlock"/> occupies.</summary>
public enum ContainerWidth
{
    /// <summary>Uses Slack's standard constrained width.</summary>
    Standard,
    /// <summary>Uses a narrower constrained width.</summary>
    Narrow,
    /// <summary>Uses a wider constrained width.</summary>
    Wide,
    /// <summary>Fills the available width.</summary>
    Full
}
