using System.Collections.Generic;

namespace SlackNet.Blocks
{
    /// <summary>
    /// This multi-select menu will populate its options with a list of Slack users visible to the current user in the active workspace.
    /// </summary>
    [SlackType("multi_users_select")]
    public class UserMultiSelectMenu : SelectMenuBase
    {
        public UserMultiSelectMenu() : base("multi_users_select") { }

        /// <summary>
        /// A list of user IDs of any valid users to be pre-selected when the menu loads.
        /// </summary>
        [IgnoreIfEmpty]
        public IList<string> InitialUsers { get; set; } = new List<string>();

        /// <summary>
        /// Specifies the maximum number of items that can be selected in the menu. Minimum number is 1.
        /// </summary>
        public int? MaxSelectedItems { get; set; }
    }

    [SlackType("multi_users_select")]
    public class UserMultiSelectAction : BlockAction
    {
        public IList<string> SelectedUsers { get; set; } = new List<string>();
    }

    [SlackType("multi_users_select")]
    public class UserMultiSelectValue : ElementValue
    {
        public IList<string> SelectedUsers { get; set; } = new List<string>();
    }
}