using System.Collections.Generic;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a file is shared into a channel, group or direct message.
    /// </summary>
    public class FileShare : MessageEvent
    {
        public IList<File> Files { get; set; } = new List<File>();
        /// <summary>
        /// Indicates whether this share happened at upload time, or some time later.
        /// </summary>
        public bool Upload { get; set; }
    }
}