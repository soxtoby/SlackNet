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

/// <summary>Names the Slack-provided icon displayed by a <see cref="SlackIcon"/>.</summary>
public enum SlackIconName
{
    /// <summary>An archive.</summary>
    Archive,
    /// <summary>A book.</summary>
    Book,
    /// <summary>A bookmark.</summary>
    Bookmark,
    /// <summary>A bot.</summary>
    Bot,
    /// <summary>A bug.</summary>
    Bug,
    /// <summary>A calendar.</summary>
    Calendar,
    /// <summary>A call.</summary>
    Call,
    /// <summary>A left-pointing caret.</summary>
    [EnumMember(Value = "caret-left")] CaretLeft,
    /// <summary>A right-pointing caret.</summary>
    [EnumMember(Value = "caret-right")] CaretRight,
    /// <summary>A check mark.</summary>
    Check,
    /// <summary>A clipboard.</summary>
    Clipboard,
    /// <summary>Code.</summary>
    Code,
    /// <summary>A comment.</summary>
    Comment,
    /// <summary>A compass.</summary>
    Compass,
    /// <summary>A copy action.</summary>
    Copy,
    /// <summary>A cube.</summary>
    Cube,
    /// <summary>A download action.</summary>
    Download,
    /// <summary>An edit action.</summary>
    Edit,
    /// <summary>Email.</summary>
    Email,
    /// <summary>A closed eye.</summary>
    [EnumMember(Value = "eye-closed")] EyeClosed,
    /// <summary>An open eye.</summary>
    [EnumMember(Value = "eye-open")] EyeOpen,
    /// <summary>A file.</summary>
    File,
    /// <summary>A flag.</summary>
    Flag,
    /// <summary>A folder.</summary>
    Folder,
    /// <summary>A gear.</summary>
    Gear,
    /// <summary>A globe.</summary>
    Globe,
    /// <summary>A heart.</summary>
    Heart,
    /// <summary>Help.</summary>
    Help,
    /// <summary>An image.</summary>
    Image,
    /// <summary>Information.</summary>
    Info,
    /// <summary>A key.</summary>
    Key,
    /// <summary>A lightbulb.</summary>
    Lightbulb,
    /// <summary>A link.</summary>
    Link,
    /// <summary>A map.</summary>
    Map,
    /// <summary>A mobile device.</summary>
    Mobile,
    /// <summary>An action that opens a new window.</summary>
    [EnumMember(Value = "new-window")] NewWindow,
    /// <summary>A pin.</summary>
    Pin,
    /// <summary>An add action.</summary>
    Plus,
    /// <summary>A refine action.</summary>
    Refine,
    /// <summary>A refresh action.</summary>
    Refresh,
    /// <summary>A rocket.</summary>
    Rocket,
    /// <summary>A save action.</summary>
    Save,
    /// <summary>A screen.</summary>
    Screen,
    /// <summary>A share action.</summary>
    Share,
    /// <summary>A sparkle.</summary>
    Sparkle,
    /// <summary>An outlined star.</summary>
    Star,
    /// <summary>A filled star.</summary>
    [EnumMember(Value = "star-filled")] StarFilled,
    /// <summary>A tag.</summary>
    Tag,
    /// <summary>A thumbs-down reaction.</summary>
    [EnumMember(Value = "thumbs-down")] ThumbsDown,
    /// <summary>A thumbs-up reaction.</summary>
    [EnumMember(Value = "thumbs-up")] ThumbsUp,
    /// <summary>A trash can.</summary>
    Trash,
    /// <summary>An upload action.</summary>
    Upload,
    /// <summary>A user.</summary>
    User,
    /// <summary>A warning.</summary>
    Warning
}
