namespace SlackNet.Events
{
    /// <summary>
    /// Sent to the current user when their Do Not Disturb settings have changed.
    /// </summary>
    public class DndUpdated : Event
    {
        public string User { get; set; }
        public OwnDndStatus DndStatus { get; set; }
    }
}