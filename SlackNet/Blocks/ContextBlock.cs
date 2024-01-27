using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Displays message context, which can include both images and text.
/// </summary>
[SlackType("context")]
public class ContextBlock : Block
{
    public ContextBlock() : base("context") { }

    /// <summary>
    /// An array of image elements and text objects. 
    /// </summary>
    public IList<IContextElement> Elements { get; set; } = [];
}