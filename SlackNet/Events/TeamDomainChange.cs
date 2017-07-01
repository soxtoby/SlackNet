namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when the team domain changes.
    /// </summary>
    public class TeamDomainChange : Event
    {
        public string Url { get; set; }
        public string Domain { get; set; }
    }
}