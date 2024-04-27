namespace SlackNet.Events;

/// <summary>
/// The <see cref="ThreadBroadcast"/> message subtype is sent when a user or bot user has indicated their reply should be broadcast to the whole channel.
/// It's a pointer or reference to the actual thread and is meant more to be informational than to fully describe the message.
/// The reference cannot contain attachments or message buttons.
/// </summary>
public class ThreadBroadcast : ReplyBroadcast
{
    public MessageEvent Root { get; set; }
}