namespace SlackNet.Blocks
{
    [SlackType("users_select")]
    public class UserSelectAction : BlockAction
    {
        public string SelectedUser { get; set; }
    }
}