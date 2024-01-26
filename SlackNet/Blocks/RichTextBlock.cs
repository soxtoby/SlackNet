using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Blocks;

[SlackType("rich_text")]
public class RichTextBlock : Block
{
    public RichTextBlock() : base("rich_text") { }

    public IList<RichTextElement> Elements { get; set; } = new List<RichTextElement>();
}

public abstract class RichTextElement
{
    protected RichTextElement(string type) => Type = type;
    
    public string Type { get; set; }
}

public class RichTextSection : RichTextElement
{
    public RichTextSection(): base("rich_text_section") { }
    
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
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

public class RichTextList : RichTextElement
{
    public RichTextList() : base("rich_text_list") { }
    
    public IList<RichTextElement> Elements { get; set; } = new List<RichTextElement>();
    public string Style { get; set; }
    public int Indent { get; set; }
    public int Border { get; set; }
}

public class RichTextQuote : RichTextElement
{
    public RichTextQuote() : base("rich_text_quote") { }
    
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
}

public class RichTextPreformatted : RichTextElement
{
    public RichTextPreformatted() : base("rich_text_preformatted") { }
    
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
}

public class RichTextStyle
{
    public bool Bold { get; set; }
    public bool Code { get; set; }
    public bool Italic { get; set; }
    public bool Strike { get; set; }
}
