namespace SlackNet.Blocks
{
    /// <summary>
    /// This select menu will populate its options with a list of public and private channels, DMs, and MPIMs visible to the current user in the active workspace.
    /// </summary>
    [SlackType("conversations_select")]
    public class ConversationSelectMenu : BlockElement, IActionElement
    {
        public ConversationSelectMenu() : base("conversations_select") { }

        /// <summary>
        /// A plain text object that defines the placeholder text shown on the menu. 
        /// </summary>
        public PlainText Placeholder { get; set; }

        /// <summary>
        /// The ID of any valid conversation to be pre-selected when the menu loads.
        /// </summary>
        public string InitialConversation { get; set; }

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