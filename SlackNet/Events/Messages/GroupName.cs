namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a private group is renamed.
    /// </summary>
    public class GroupName : Message
    {
        public string OldName { get; set; }
        public string Name { get; set; }
    }
}