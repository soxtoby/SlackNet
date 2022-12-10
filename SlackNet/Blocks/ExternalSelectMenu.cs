using SlackNet.Interaction;

namespace SlackNet.Blocks;

/// <summary>
/// This select menu will load its options from an external data source, allowing for a dynamic list of options.
/// </summary>
[SlackType("external_select")]
public class ExternalSelectMenu : ExternalSelectMenuBase
{
    public ExternalSelectMenu() : base("external_select") { }

    /// <summary>
    /// A single <see cref="Option"/> that exactly matches one of the options within the <see cref="BlockOptionsResponse.Options"/>
    /// or <see cref="BlockOptionsResponse.OptionGroups"/> loaded from the external data source.
    /// This option will be selected when the menu initially loads.
    /// </summary>
    public Option InitialOption { get; set; }
}

[SlackType("external_select")]
public class ExternalSelectAction : BlockAction
{
    public Option SelectedOption { get; set; }
}

[SlackType("external_select")]
public class ExternalSelectValue : ElementValue
{
    public Option SelectedOption { get; set; }
}