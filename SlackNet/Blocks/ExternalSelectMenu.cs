using SlackNet.Interaction;

namespace SlackNet.Blocks
{
    /// <summary>
    /// This select menu will load its options from an external data source, allowing for a dynamic list of options.
    /// </summary>
    [SlackType("external_select")]
    public class ExternalSelectMenu : BlockElement, IActionElement
    {
        public ExternalSelectMenu() : base("external_select") { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the menu. 
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// A single <see cref="Option"/> that exactly matches one of the options within the <see cref="BlockOptionsResponse.Options"/>
        /// or <see cref="BlockOptionsResponse.OptionGroups"/> loaded from the external data source.
        /// This option will be selected when the menu initially loads.
        /// </summary>
        public Option InitialOption { get; set; }

        /// <summary>
        /// When the typeahead field is used, a request will be sent on every character change.
        /// If you prefer fewer requests or more fully ideated queries, use this to tell Slack the fewest number of typed characters required before dispatch.
        /// </summary>
        public int? MinQueryLength { get; set; }

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