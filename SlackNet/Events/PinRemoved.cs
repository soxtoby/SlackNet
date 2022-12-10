namespace SlackNet.Events;

/// <summary>
/// sent to all members of a channel when an item is un-pinned from that channel.
/// </summary>
public class PinRemoved : Event
{
    public string User { get; set; }
    public string ChannelId { get; set; }
    public PinnedItem Item { get; set; }
    /// <summary>
    /// Indicates that there are other pinned items in the channel.
    /// </summary>
    public bool HasPins { get; set; }
}