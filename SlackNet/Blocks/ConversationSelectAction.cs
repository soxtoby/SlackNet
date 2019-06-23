namespace SlackNet.Blocks
{
    [SlackType("conversations_select")]
    public class ConversationSelectAction : BlockAction
    {
        public string SelectedConversation { get; set; }
    }
}