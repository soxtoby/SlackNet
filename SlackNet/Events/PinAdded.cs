namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all members of a channel when an item is pinned in that channel.
    /// </summary>
    public class PinAdded : Event
    {
        public string User { get; set; }
        public string ChannelId { get; set; }
        public PinnedItem Item { get; set; }
        public string EventTs { get; set; }
    }
}