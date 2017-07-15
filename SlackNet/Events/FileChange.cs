namespace SlackNet.Events
{
    /// <summary>
    /// Sent when any property of a file is changed. It is sent to all connected clients for all users that have permission to see the file.
    /// </summary>
    public class FileChange : Event
    {
        public File File { get; set; }
    }
}