namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a team member updates their profile or data.
    /// </summary>
    public class UserChange : Event
    {
        public User User { get; set; }
    }
}