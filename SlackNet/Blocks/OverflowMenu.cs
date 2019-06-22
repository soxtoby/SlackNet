using System.Collections.Generic;

namespace SlackNet.Blocks
{
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
    public class OverflowMenu : BlockElement, IActionElement
    {
        public OverflowMenu() : base("overflow") { }

        /// <summary>
        /// A list of option objects to display in the menu.
        /// </summary>
        public IList<OverflowOption> Options { get; set; } = new List<OverflowOption>();

        /// <summary>
        /// Defines an optional confirmation dialog that appears after a menu item is selected.
        /// </summary>
        public ConfirmationDialog Confirm { get; set; }

        /// <summary>
        /// An identifier for this action. You can use this when you receive an interaction payload to identify the source of the action.
        /// Should be unique among all other <see cref="ActionId"/>s used elsewhere by your app. 
        /// </summary>
        public string ActionId { get; set; }
    }
}