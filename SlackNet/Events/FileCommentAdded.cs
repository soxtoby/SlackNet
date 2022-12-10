namespace SlackNet.Events;

/// <summary>
/// Sent when a comment is added to file. It is sent to all connected clients for users who can see the file.
/// </summary>
public class FileCommentAdded : Event
{
    public string FileId { get; set; }
    public FileId File { get; set; }
    public FileComment Comment { get; set; }
}