using SlackNet.Objects;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a file is shared into a channel, group or direct message.
    /// </summary>
    public class FileShare : MessageEvent
    {
        public File File { get; set; }
        /// <summary>
        /// Indicates whether this share happened at upload time, or some time later.
        /// </summary>
        public bool Upload { get; set; }
    }
}