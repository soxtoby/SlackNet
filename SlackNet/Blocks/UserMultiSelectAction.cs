using System.Collections.Generic;

namespace SlackNet.Blocks
{
    [SlackType("multi_users_select")]
    public class UserMultiSelectAction
    {
        public IList<string> SelectedUsers { get; set; } = new List<string>();
    }
}