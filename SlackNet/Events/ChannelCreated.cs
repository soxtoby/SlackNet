#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet.Events;

/// <summary>
/// Sent to all connections for a team when a new team channel is created.
/// </summary>
public class ChannelCreated : Event
{
    public CreatedChannel Channel { get; set; }
}

public class CreatedChannel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Created { get; set; }
    [JsonIgnore]
    public DateTime CreatedDate => Created.ToDateTime().GetValueOrDefault();
    public string Creator { get; set; }
}