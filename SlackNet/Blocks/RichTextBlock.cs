using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet.Blocks
{
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
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("channel")]
    public class RichTextChannel : RichTextSectionElement
    {
        public string ChannelId { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("user")]
    public class RichTextUser
    {
        public string UserId { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("emoji")]
    public class RichTextEmoji : RichTextSectionElement
    {
        public string Name { get; set; }
    }

    [SlackType("link")]
    public class RichTextLink : RichTextSectionElement
    {
        public string Url { get; set; }
        public string Text { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("team")]
    public class RichTextTeam : RichTextSectionElement
    {
        public string TeamId { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("usergroup")]
    public class RichTextUserGroup : RichTextSectionElement
    {
        [JsonProperty("usergroup_id")]
        public string UserGroupId { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("date")]
    public class RichTextDate : RichTextSectionElement
    {
        public string Text { get; set; }
        public string Timestamp { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    [SlackType("broadcast")]
    public class RichTextBroadcast : RichTextSectionElement
    {
        public string Range { get; set; }
        public RichTextStyle Style { get; set; } = new RichTextStyle();
    }

    public class RichTextList : RichTextElement
    {
        public IList<RichTextElement> Elements { get; set; } = new List<RichTextElement>();
        public string Style { get; set; }
        public int Indent { get; set; }
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
}