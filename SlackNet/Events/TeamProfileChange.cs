namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a Team Admin updates the field definitions in the team profile.
    /// Only the modified field definitions are included in the payload. 
    /// </summary>
    public class TeamProfileChange : Event
    {
        public TeamProfile Profile { get; set; }
    }
}