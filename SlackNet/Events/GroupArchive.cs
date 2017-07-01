namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for members of a private channel when that private channel is archived.
    /// </summary>
    public class GroupArchive : Event
    {
        public string Channel { get; set; }
    }
}