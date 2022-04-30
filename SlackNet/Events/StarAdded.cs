namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connected clients for the authenticated user who starred the item when an item is starred.
    /// </summary>
    public class StarAdded : Event
    {
        public string User { get; set; }
        public StarredItem Item { get; set; }
    }
}