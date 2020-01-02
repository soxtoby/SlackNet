using System.Collections.Generic;

namespace SlackNet.Blocks
{
    /// <summary>
    /// A multi-select menu allows a user to select multiple items from a list of options.
    /// </summary>
    [SlackType("multi_static_select")]
    public class StaticMultiSelectMenu : StaticSelectMenuBase
    {
        public StaticMultiSelectMenu() : base("multi_static_select") { }

        /// <summary>
        /// A list of <see cref="Option"/>s that exactly match one or more of the options within <see cref="StaticSelectMenuBase.Options"/> or <see cref="StaticSelectMenuBase.OptionGroups"/>.
        /// These options will be selected when the menu initially loads.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<Option> InitialOptions { get; set; } = new List<Option>();

        /// <summary>
        /// Specifies the maximum number of items that can be selected in the menu. Minimum number is 1.
        /// </summary>
        public int? MaxSelectedItems { get; set; }
    }
}