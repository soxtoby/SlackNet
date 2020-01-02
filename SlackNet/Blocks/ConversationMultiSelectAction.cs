using System.Collections.Generic;

namespace SlackNet.Blocks
{
    [SlackType("multi_conversations_select")]
    public class ConversationMultiSelectAction
    {
        public IList<string> SelectedConversations { get; set; } = new List<string>();
    }
}