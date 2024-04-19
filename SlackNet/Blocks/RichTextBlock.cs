using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Blocks;

/// <summary>
/// Displays formatted, structured representation of text.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#rich_text">Slack documentation</a> for more information.</remarks>
[SlackType("rich_text")]
public class RichTextBlock : Block
{
    public RichTextBlock() : base("rich_text") { }

    public IList<RichTextElement> Elements { get; set; } = [];
}

public abstract class RichTextElement
{
    protected RichTextElement(string type) => Type = type;
    
    public string Type { get; set; }
}

/// <summary>
/// Section element.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#rich_text_section">Slack documentation</a> for more information.</remarks>
public class RichTextSection : RichTextElement
{
    public RichTextSection(): base("rich_text_section") { }
    
    public IList<RichTextSectionElement> Elements { get; set; } = [];
}

public abstract class RichTextSectionElement
{
    protected RichTextSectionElement(string type) => Type = type;
    
    public string Type { get; set; }
}

[SlackType("text")]
public class RichTextText : RichTextSectionElement
{
    public RichTextText() : base("text") { }
    
    public string Text { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("channel")]
public class RichTextChannel : RichTextSectionElement
{
    public RichTextChannel() : base("channel") { }
    
    public string ChannelId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("user")]
public class RichTextUser : RichTextSectionElement
{
    public RichTextUser() : base("user") { }
    
    public string UserId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("emoji")]
public class RichTextEmoji : RichTextSectionElement
{
    public RichTextEmoji() : base("emoji") { }
    
    public string Name { get; set; }
    public string Unicode { get; set; } 
    public int? SkinTone { get; set; }
}

[SlackType("link")]
public class RichTextLink : RichTextSectionElement
{
    public RichTextLink() : base("link") { }
    
    public string Url { get; set; }
    public string Text { get; set; }
    public RichTextStyle Style { get; set; } = new();
    public bool? Unsafe { get; set; }
}

[SlackType("team")]
public class RichTextTeam : RichTextSectionElement
{
    public RichTextTeam() : base("team") { }
    
    public string TeamId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("usergroup")]
public class RichTextUserGroup : RichTextSectionElement
{
    public RichTextUserGroup() : base("usergroup") { }
    
    [JsonProperty("usergroup_id")]
    public string UserGroupId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("date")]
public class RichTextDate : RichTextSectionElement
{
    public RichTextDate() : base("date") { }
    
    public string Text { get; set; }
    public string Timestamp { get; set; }
    /// <summary>
    /// See https://api.slack.com/reference/surfaces/formatting#date-formatting for more information.
    /// </summary>
    public string Format { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("broadcast")]
public class RichTextBroadcast : RichTextSectionElement
{
    public RichTextBroadcast() : base("broadcast") { }
    
    public string Range { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

/// <summary>
/// List element.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#rich_text_list">Slack documentation</a> for more information.</remarks>
public class RichTextList : RichTextElement
{
    public RichTextList() : base("rich_text_list") { }
    
    /// <summary>
    /// An array of <see cref="RichTextSection"/> objects.
    /// </summary>
    public IList<RichTextSection> Elements { get; set; } = [];
    
    /// <summary>
    /// Either <see cref="RichTextListStyle.Bullet"/> or <see cref="RichTextListStyle.Ordered"/>, the latter meaning a numbered list.
    /// </summary>
    public RichTextListStyle Style { get; set; }
    
    /// <summary>
    /// Levels of indentation.
    /// </summary>
    public int Indent { get; set; }
    
    /// <summary>
    /// Number of pixels to offset the list.
    /// </summary>
    public int Offset { get; set; }
    
    /// <summary>
    /// Number of pixels of border thickness.
    /// </summary>
    public int Border { get; set; }
}

public enum RichTextListStyle
{
    Bullet,
    Ordered
}

/// <summary>
/// Quote element.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#rich_text_quote">Slack documentation</a> for more information.</remarks>
public class RichTextQuote : RichTextElement
{
    public RichTextQuote() : base("rich_text_quote") { }
    
    /// <summary>
    /// An array of rich text elements.
    /// </summary>
    public IList<RichTextSectionElement> Elements { get; set; } = [];

    /// <summary>
    /// Number of pixels of border thickness.
    /// </summary>
    public int Border { get; set; }
}

/// <summary>
/// Preformatted code block element.
/// </summary>
/// <remarks>See the <a href="https://api.slack.com/reference/block-kit/blocks#rich_text_preformatted">Slack documentation</a> for more information.</remarks>
public class RichTextPreformatted : RichTextElement
{
    public RichTextPreformatted() : base("rich_text_preformatted") { }
    
    /// <summary>
    /// An array of rich text elements.
    /// </summary>
    public IList<RichTextSectionElement> Elements { get; set; } = [];

    /// <summary>
    /// Number of pixels of border thickness.
    /// </summary>
    public int Border { get; set; }
}

/// <summary>
/// Rich text styling. Note that not all properties are supported by all elements.
/// </summary>
public class RichTextStyle
{
    [IgnoreIfDefault]
    public bool Bold { get; set; }
    [IgnoreIfDefault]
    public bool Italic { get; set; }
    [IgnoreIfDefault]
    public bool Strike { get; set; }
    [IgnoreIfDefault]
    public bool Code { get; set; }
    [IgnoreIfDefault]
    public bool Highlight { get; set; }
    [IgnoreIfDefault]
    public bool ClientHighlight { get; set; }
    [IgnoreIfDefault]
    public bool Unlink { get; set; }
}