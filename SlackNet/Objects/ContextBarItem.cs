using System.Runtime.Serialization;

namespace SlackNet;

/// <summary>
/// An item displayed in a code channel's context bar, for quick orientation and quick actions for the people in the channel.
/// An item can be a link (<see cref="Url"/>) or an action (<see cref="ContextBarItemType.Action"/>), but not both.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/ai/slack-code/code-channel-properties/#context-bar-items">Slack documentation</a> for more information.</remarks>
public class ContextBarItem
{
    /// <summary>
    /// Unique identifier for the item within the channel.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Display text.
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// Icon shown beside the label.
    /// </summary>
    public ContextBarIcon? Icon { get; set; }

    /// <summary>
    /// Makes the item a link to this URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Whether the item is informational or interactive.
    /// </summary>
    public ContextBarItemType ItemType { get; set; }
}

/// <summary>
/// An icon shown beside the label of a <see cref="ContextBarItem"/>.
/// </summary>
public enum ContextBarIcon
{
    Branch,
    Folder,
    Hierarchy,
    [EnumMember(Value = "life-ring")] LifeRing,
    Link,
    Globe,
    Terminal,
    Code,
    Search,
    Lock
}

/// <summary>
/// Whether a <see cref="ContextBarItem"/> is informational or interactive.
/// </summary>
public enum ContextBarItemType
{
    /// <summary>
    /// Informational.
    /// </summary>
    Info,

    /// <summary>
    /// Interactive. Clicking it delivers a <c>code_channel_action</c> event to your app, rather than linking out.
    /// </summary>
    Action
}
