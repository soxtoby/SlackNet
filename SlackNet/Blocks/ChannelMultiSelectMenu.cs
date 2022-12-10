using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// This multi-select menu will populate its options with a list of public channels visible to the current user in the active workspace.
/// </summary>
[SlackType("multi_channels_select")]
public class ChannelMultiSelectMenu : SelectMenuBase
{
    public ChannelMultiSelectMenu() : base("multi_channels_select") { }

    /// <summary>
    /// A list of one or more IDs of any valid public channel to be pre-selected when the menu loads.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<string> InitialChannels { get; set; } = new List<string>();

    /// <summary>
    /// Specifies the maximum number of items that can be selected in the menu. Minimum number is 1.
    /// </summary>
    public int? MaxSelectedItems { get; set; }
}

[SlackType("multi_channels_select")]
public class ChannelMultiSelectAction : BlockAction
{
    public IList<string> SelectedChannels { get; set; } = new List<string>();
}

[SlackType("multi_channels_select")]
public class ChannelMultiSelectValue : ElementValue
{
    public IList<string> SelectedChannels { get; set; } = new List<string>();
}