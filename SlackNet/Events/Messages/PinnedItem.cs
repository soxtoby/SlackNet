namespace SlackNet.Events;

/// <summary>
/// Sent when an item is pinned to a channel.
/// </summary>
public class PinnedItem : MessageEvent
{
    public PinnedItemType ItemType { get; set; }
    public SlackNet.PinnedItem Item { get; set; }
}