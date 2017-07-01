namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when the topic for a private group is changed.
    /// </summary>
    public class GroupTopic : Message
    {
        public string Topic { get; set; }
    }
}