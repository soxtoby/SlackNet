using System.Collections.Generic;

namespace SlackNet.Blocks;

/// <summary>
/// This is like a cross between a button and a select menu - when a user clicks on this overflow button,
/// they will be presented with a list of options to choose from. Unlike the select menu, there is no typeahead field,
/// and the button always appears with an ellipsis ("…") rather than customisable text.
///
/// As such, it is usually used if you want a more compact layout than a select menu,
/// or to supply a list of less visually important actions after a row of buttons.
/// You can also specify simple URL links as overflow menu options, instead of actions.
/// </summary>
[SlackType("overflow")]
public class OverflowMenu : ActionElement
{
    public OverflowMenu() : base("overflow") { }

    /// <summary>
    /// A list of option objects to display in the menu.
    /// </summary>
    public IList<OverflowOption> Options { get; set; } = new List<OverflowOption>();
}

[SlackType("overflow")]
public class OverflowAction : BlockAction
{
    public Option SelectedOption { get; set; }
}

[SlackType("overflow")]
public class OverflowValue : ElementValue
{
    public Option SelectedOption { get; set; }
}