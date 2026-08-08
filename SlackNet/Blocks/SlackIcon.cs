using System.Runtime.Serialization;

namespace SlackNet.Blocks;

/// <summary>
/// Defines Slack icon information for use in a <see cref="CardBlock"/>.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/block-kit/composition-objects/slack-icon-object/">Slack documentation</a> for more information.</remarks>
public class SlackIcon
{
    /// <summary>
    /// The type of composition object. Always <c>icon</c>.
    /// </summary>
    public string Type { get; } = "icon";

    /// <summary>
    /// The name of the Slack icon.
    /// </summary>
    public SlackIconName Name { get; set; }
}

public enum SlackIconName
{
    Archive,
    Book,
    Bookmark,
    Bot,
    Bug,
    Calendar,
    Call,
    [EnumMember(Value = "caret-left")] CaretLeft,
    [EnumMember(Value = "caret-right")] CaretRight,
    Check,
    Clipboard,
    Code,
    Comment,
    Compass,
    Copy,
    Cube,
    Download,
    Edit,
    Email,
    [EnumMember(Value = "eye-closed")] EyeClosed,
    [EnumMember(Value = "eye-open")] EyeOpen,
    File,
    Flag,
    Folder,
    Gear,
    Globe,
    Heart,
    Help,
    Image,
    Info,
    Key,
    Lightbulb,
    Link,
    Map,
    Mobile,
    [EnumMember(Value = "new-window")] NewWindow,
    Pin,
    Plus,
    Refine,
    Refresh,
    Rocket,
    Save,
    Screen,
    Share,
    Sparkle,
    Star,
    [EnumMember(Value = "star-filled")] StarFilled,
    Tag,
    [EnumMember(Value = "thumbs-down")] ThumbsDown,
    [EnumMember(Value = "thumbs-up")] ThumbsUp,
    Trash,
    Upload,
    User,
    Warning
}
