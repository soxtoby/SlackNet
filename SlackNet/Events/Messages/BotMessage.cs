using System.Collections.Generic;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a message is sent to a channel by an integration "bot".
    /// It is like a normal user message, except it has no associated user.
    /// </summary>
    public class BotMessage : MessageEvent
    {
        public string BotId { get; set; }
        public string Username { get; set; }
        public Dictionary<string, string> Icons { get; set; }
    }
}