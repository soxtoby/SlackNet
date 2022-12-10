namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a team channel is renamed.
/// </summary>
public class ChannelRename : Event
{
    public RenamedChannel Channel { get; set; }
}

public class RenamedChannel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Created { get; set; }
}