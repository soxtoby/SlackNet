using System.Collections.Generic;

namespace SlackNet.Blocks
{
    /// <summary>
    /// This is the simplest form of select menu, with a static list of options passed in when defining the element.
    /// </summary>
    [SlackType("static_select")]
    public class StaticSelectMenu : BlockElement, IActionElement
    {
        public StaticSelectMenu() : base("static_select") { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the menu. 
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// An array of <see cref="Option"/> objects. If <see cref="OptionGroups"/> is specified, this field should not be.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<Option> Options { get; set; } = new List<Option>();

        /// <summary>
        /// An array of <see cref="OptionGroup"/> objects. If <see cref="Options"/> is specified, this field should not be.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<OptionGroup> OptionGroups { get; set; } = new List<OptionGroup>();

        /// <summary>
        /// A single <see cref="Option"/> that exactly matches one of the options within <see cref="Options"/> or <see cref="OptionGroups"/>.
        /// This option will be selected when the menu initially loads.
        /// </summary>
        public Option InitialOption { get; set; }

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