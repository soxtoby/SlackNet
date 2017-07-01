namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for members of a private channel when that private channel is unarchived.
    /// </summary>
    public class GroupUnarchive : Event
    {
        public string Channel { get; set; }
    }
}