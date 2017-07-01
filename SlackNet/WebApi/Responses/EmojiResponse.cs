using System.Collections.Generic;

namespace SlackNet.WebApi.Responses
{
    class EmojiResponse
    {
        public Dictionary<string, string> Emoji { get; } = new Dictionary<string, string>();
    }
}