namespace SlackNet.Blocks;

/// <summary>
/// This is the simplest form of select menu, with a static list of options passed in when defining the element.
/// </summary>
[SlackType("static_select")]
public class StaticSelectMenu : StaticSelectMenuBase
{
    public StaticSelectMenu() : base("static_select") { }

    /// <summary>
    /// A single <see cref="Option"/> that exactly matches one of the options within <see cref="StaticSelectMenuBase.Options"/> or <see cref="StaticSelectMenuBase.OptionGroups"/>.
    /// This option will be selected when the menu initially loads.
    /// </summary>
    public Option InitialOption { get; set; }
}

[SlackType("static_select")]
public class StaticSelectAction : BlockAction
{
    public Option SelectedOption { get; set; }
}

[SlackType("static_select")]
public class StaticSelectValue : ElementValue
{
    public Option SelectedOption { get; set; }
}