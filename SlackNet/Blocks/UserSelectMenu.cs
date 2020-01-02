namespace SlackNet.Blocks
{
    /// <summary>
    /// This select menu will populate its options with a list of Slack users visible to the current user in the active workspace.
    /// </summary>
    [SlackType("users_select")]
    public class UserSelectMenu : SelectMenuBase
    {
        public UserSelectMenu() : base("users_select") { }

        /// <summary>
        /// The user ID of any valid user to be pre-selected when the menu loads.
        /// </summary>
        public string InitialUser { get; set; }
    }
}