namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a private group is unarchived.
    /// </summary>
    [SlackType("group_unarchive")]
    public class GroupUnarchiveMessage : MessageEvent { }
}