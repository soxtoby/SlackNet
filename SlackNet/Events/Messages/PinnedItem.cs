using SlackNet.Objects;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when an item is pinned to a channel.
    /// </summary>
    public class PinnedItem : Message
    {
        public PinnedItemType ItemType { get; set; }
        public Objects.PinnedItem Item { get; set; }
    }
}