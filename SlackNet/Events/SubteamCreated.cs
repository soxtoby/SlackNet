namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a new User Group is created.
    /// </summary>
    public class SubteamCreated : Event
    {
        public UserGroup Subteam { get; set; }
    }
}