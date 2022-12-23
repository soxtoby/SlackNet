﻿using System.Collections.Generic;
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
    public string Type { get; set; }
}

public class RichTextSection : RichTextElement
{
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
}

public abstract class RichTextSectionElement
{
    public string Type { get; set; }
}

[SlackType("text")]
public class RichTextText : RichTextSectionElement
{
    public string Text { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("channel")]
public class RichTextChannel : RichTextSectionElement
{
    public string ChannelId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("user")]
public class RichTextUser : RichTextSectionElement
{
    public string UserId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("emoji")]
public class RichTextEmoji : RichTextSectionElement
{
    public string Name { get; set; }
    public string Unicode { get; set; }
    public int? SkinTone { get; set; }
}

[SlackType("link")]
public class RichTextLink : RichTextSectionElement
{
    public string Url { get; set; }
    public string Text { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("team")]
public class RichTextTeam : RichTextSectionElement
{
    public string TeamId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("usergroup")]
public class RichTextUserGroup : RichTextSectionElement
{
    [JsonProperty("usergroup_id")]
    public string UserGroupId { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("date")]
public class RichTextDate : RichTextSectionElement
{
    public string Text { get; set; }
    public string Timestamp { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

[SlackType("broadcast")]
public class RichTextBroadcast : RichTextSectionElement
{
    public string Range { get; set; }
    public RichTextStyle Style { get; set; } = new();
}

public class RichTextList : RichTextElement
{
    public IList<RichTextElement> Elements { get; set; } = new List<RichTextElement>();
    public string Style { get; set; }
    public int Indent { get; set; }
    public int Border { get; set; }
}

public class RichTextQuote : RichTextElement
{
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
}

public class RichTextPreformatted : RichTextElement
{
    public IList<RichTextSectionElement> Elements { get; set; } = new List<RichTextSectionElement>();
}

public class RichTextStyle
{
    public bool Bold { get; set; }
    public bool Code { get; set; }
    public bool Italic { get; set; }
    public bool Strike { get; set; }
}
