namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when the purpose for a private group is changed.
    /// </summary>
    public class GroupPurpose : MessageEvent
    {
        public string Purpose { get; set; }
    }
}