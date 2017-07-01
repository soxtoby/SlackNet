namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a Team Admin reorders the field definitions in the team profile.
    /// </summary>
    public class TeamProfileReorder : Event
    {
        public ReorderedProfile Profile { get; set; }
    }
}