namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a user when that user joins a private channel.
    /// </summary>
    public class GroupJoined : Event
    {
        public Channel Channel { get; set; }
    }
}