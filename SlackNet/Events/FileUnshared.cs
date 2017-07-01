using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a file is unshared. It is sent to all connected clients for all users that had permission to see the file.
    /// </summary>
    public class FileUnshared : Event
    {
        public File File { get; set; }
    }
}