namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all members of a private channel when it is deleted.
    /// </summary>
    public class GroupDeleted : Event
    {
        public string Channel { get; set; }
    }
}