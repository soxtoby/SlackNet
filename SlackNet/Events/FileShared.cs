namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a file is shared. It is sent to all connected clients for all users that have permission to see the file.
    /// </summary>
    public class FileShared : Event
    {
        public File File { get; set; }
    }
}