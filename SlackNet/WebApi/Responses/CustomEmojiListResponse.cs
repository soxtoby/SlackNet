using System.Collections.Generic;

namespace SlackNet.WebApi;

public class CustomEmojiListResponse
{
    /// <summary>
    /// The custom emoji, keyed by name.
    /// </summary>
    public IDictionary<string, CustomEmoji> Emoji { get; set; } = new Dictionary<string, CustomEmoji>();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
