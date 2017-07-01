using SlackNet.Objects;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// When an item is un-pinned from a channel, an <see cref="UnpinnedItem"/> message is sent via slackbot to the user that initially pinned the item.
    /// The message will only be sent if the item was un-pinned by a different user.
    /// </summary>
    public class UnpinnedItem : Message
    {
        public PinnedItemType ItemType { get; set; }
        public Objects.PinnedItem Item { get; set; }
    }
}