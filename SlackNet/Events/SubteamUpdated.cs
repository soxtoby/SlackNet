namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when an existing User Group is updated.
    /// This event is triggered for changes to the User Group information (name, description, or handle) as well as the members of the group.
    /// </summary>
    public class SubteamUpdated : Event
    {
        public UserGroup Subteam { get; set; }
    }
}