namespace SlackNet.Events;

/// <summary>
/// Sent when a file comment is deleted. It is sent to all connected clients for users who can see the file.
/// </summary>
public class FileCommentDeleted : Event
{
    public string FileId { get; set; }
    public FileId File { get; set; }
    /// <summary>
    /// The ID of the deleted comment.
    /// </summary>
    public string Comment { get; set; }
}