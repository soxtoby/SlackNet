using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when that team's custom emoji is updated.
    /// </summary>
    public class EmojiChanged : Event
    {
        public string Subtype { get; set; }
        public IList<string> Names { get; set; }
    }
}