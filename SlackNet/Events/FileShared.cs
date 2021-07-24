namespace SlackNet.Events
{
    /// <summary>
    /// Sent when a file is shared. It is sent to all connected clients for all users that have permission to see the file.
    /// </summary>
    public class FileShared : Event
    {
        public string FileId { get; set; }
        public string ChannelId { get; set; }
        public string UserId { get; set; }      
        public FileId File { get; set; }
    }
}
