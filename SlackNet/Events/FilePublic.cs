using SlackNet.Objects;

namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a file is made public. It is sent to all connected clients for all users that have permission to see the file.
    /// </summary>
    public class FilePublic : Event
    {
        public File File { get; set; }
    }
}