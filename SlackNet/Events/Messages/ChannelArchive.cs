using System.Collections.Generic;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a channel is archived.
    /// </summary>
    public class ChannelArchive : Message
    {
        public IList<string> Members { get; set; } = new List<string>();
    }
}