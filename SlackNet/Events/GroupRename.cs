namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for members of a private channel when that channel is renamed.
    /// </summary>
    public class GroupRename : Event
    {
        public RenamedChannel Channel { get; set; }
    }
}