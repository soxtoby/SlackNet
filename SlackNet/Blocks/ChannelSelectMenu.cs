namespace SlackNet.Blocks
{
    /// <summary>
    /// This select menu will populate its options with a list of public channels visible to the current user in the active workspace.
    /// </summary>
    [SlackType("channels_select")]
    public class ChannelSelectMenu : SelectMenuBase
    {
        public ChannelSelectMenu() : base("channels_select") { }

        /// <summary>
        /// The ID of any valid public channel to be pre-selected when the menu loads.
        /// </summary>
        public string InitialChannel { get; set; }
    }

    [SlackType("channels_select")]
    public class ChannelSelectAction : BlockAction
    {
        public string SelectedChannel { get; set; }
    }

    [SlackType("channels_select")]
    public class ChannelSelectValue : ElementValue
    {
        public string SelectedChannel { get; set; }
    }
}