namespace SlackNet.Events
{
    /// <summary>
    /// Sent to all connections for a team when a slash command for that team is added, removed or changed.
    /// This functionality is only used by Slack's web client. The other APIs required to support slash command metadata are currently unstable.
    /// Until they are released other clients should ignore this event.
    /// </summary>
    public class CommandsChanged : Event { }
}