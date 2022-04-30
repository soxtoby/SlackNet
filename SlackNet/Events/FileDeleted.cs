namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connected clients for a team when a file is deleted.
    /// </summary>
    public class FileDeleted : Event
    {
        public string FileId { get; set; }
    }
}