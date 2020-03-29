using System.Collections.Generic;
using SlackNet.Interaction;

namespace SlackNet.Blocks
{
    /// <summary>
    /// This menu will load its options from an external data source, allowing for a dynamic list of options.
    /// </summary>
    [SlackType("multi_external_select")]
    public class ExternalMultiSelectMenu : ExternalSelectMenuBase
    {
        public ExternalMultiSelectMenu() : base("multi_external_select") { }

        /// <summary>
        /// A list of <see cref="Option"/>s that exactly match one or more of the options within the <see cref="BlockOptionsResponse.Options"/>
        /// or <see cref="BlockOptionsResponse.OptionGroups"/> loaded from the external data source.
        /// These options will be selected when the menu initially loads.
        /// </summary>
        public IList<Option> InitialOptions { get; set; } = new List<Option>();

        /// <summary>
        /// Specifies the maximum number of items that can be selected in the menu. Minimum number is 1.
        /// </summary>
        public int? MaxSelectedItems { get; set; }
    }

    [SlackType("multi_external_select")]
    public class ExternalMultiSelectAction
    {
        public IList<Option> SelectedOptions { get; set; } = new List<Option>();
    }

    [SlackType("multi_external_select")]
    public class ExternalMultiSelectValue
    {
        public IList<Option> SelectedOptions { get; set; } = new List<Option>();
    }
}