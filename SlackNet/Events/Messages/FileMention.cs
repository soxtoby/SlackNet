namespace SlackNet.Events;

/// <summary>
/// Sent when a file is mentioned in a channel, group or direct message.
/// </summary>
public class FileMention : MessageEvent
{
    public File File { get; set; }
}