namespace SlackNet.Events;

/// <summary>
/// Sent when any property of a file is changed. It is sent to all connected clients for all users that have permission to see the file.
/// </summary>
public class FileChange : Event
{
    public string FileId { get; set; }
    public FileId File { get; set; }
}