namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a user's Do Not Disturb settings have changed.
    /// </summary>
    public class DndUpdatedUser : Event
    {
        public string User { get; set; }
        public DndStatus DndStatus { get; set; }
    }
}