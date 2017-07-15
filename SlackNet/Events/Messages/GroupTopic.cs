namespace SlackNet.Events
{
    /// <summary>
    /// Sent when the topic for a private group is changed.
    /// </summary>
    public class GroupTopic : MessageEvent
    {
        public string Topic { get; set; }
    }
}