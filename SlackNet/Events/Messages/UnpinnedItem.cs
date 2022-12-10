namespace SlackNet.Events;

/// <summary>
/// When an item is un-pinned from a channel, an <see cref="UnpinnedItem"/> message is sent via slackbot to the user that initially pinned the item.
/// The message will only be sent if the item was un-pinned by a different user.
/// </summary>
public class UnpinnedItem : MessageEvent
{
    public PinnedItemType ItemType { get; set; }
    public SlackNet.PinnedItem Item { get; set; }
}