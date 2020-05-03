namespace SlackNet.Blocks
{
    /// <summary>
    /// This select menu will populate its options with a list of public and private channels, DMs, and MPIMs visible to the current user in the active workspace.
    /// </summary>
    [SlackType("conversations_select")]
    public class ConversationSelectMenu : SelectMenuBase
    {
        public ConversationSelectMenu() : base("conversations_select") { }

        /// <summary>
        /// The ID of any valid conversation to be pre-selected when the menu loads.
        /// </summary>
        public string InitialConversation { get; set; }

        /// <summary>
        /// A <see cref="ConversationFilter"/> object that reduces the list of available conversations using the specified criteria.
        /// </summary>
        public ConversationFilter Filter { get; set; }

        /// <summary>
        /// Pre-populates the select menu with the conversation that the user was viewing when they opened the modal, if available.
        /// If <see cref="InitialConversation"/> is also supplied, it will be ignored.
        /// </summary>
        public bool DefaultToCurrentConversation { get; set; }
    }

    [SlackType("conversations_select")]
    public class ConversationSelectAction : BlockAction
    {
        public string SelectedConversation { get; set; }
    }

    [SlackType("conversations_select")]
    public class ConversationSelectValue : ElementValue
    {
        public string SelectedConversation { get; set; }
    }
}