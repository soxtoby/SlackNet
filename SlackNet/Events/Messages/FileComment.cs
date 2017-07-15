using SlackNet.Objects;

namespace SlackNet.Events.Messages
{
    /// <summary>
    /// Sent when a new comment is added to a file shared into a channel, group or direct message.
    /// </summary>
    public class FileComment : MessageEvent
    {
        public File File { get; set; }
        public FileComment Comment { get; set; }
    }
}