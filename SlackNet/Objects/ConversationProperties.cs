using System.Collections.Generic;

namespace SlackNet;

public class ConversationProperties
{
    /// <summary>
    /// Tabs in the conversation. Slack omits this property when there are no tabs.
    /// </summary>
    public IList<ConversationTab> Tabs { get; set; } = [];
}

public class ConversationTab
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
}
