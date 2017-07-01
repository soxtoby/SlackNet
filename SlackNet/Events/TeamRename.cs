namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when an admin changes the team name.
    /// </summary>
    public class TeamRename : Event
    {
        public string Name { get; set; }
    }
}