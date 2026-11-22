using System.Collections.Generic;

namespace SlackNet.WebApi;

class EmojiResponse
{
    public Dictionary<string, string> Emoji { get; set; } = new();
}