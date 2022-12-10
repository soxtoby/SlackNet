using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// Provides a way to group options in a select menu.
/// </summary>
public class OptionGroup
{
    /// <summary>
    /// A plain text object that defines the label shown above this group of options.
    /// </summary>
    public PlainText Label { get; set; }

    /// <summary>
    /// A list of <see cref="Option"/> objects that belong to this specific group.
    /// </summary>
    public IList<Option> Options { get; set; }
}