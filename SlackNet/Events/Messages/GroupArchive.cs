using System.Collections.Generic;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a private group is archived.
    /// </summary>
    public class GroupArchive : Message
    {
        public IList<string> Members { get; set; } = new List<string>();
    }
}