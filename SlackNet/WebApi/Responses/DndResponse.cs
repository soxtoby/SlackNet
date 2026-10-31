using System;
using Newtonsoft.Json;

namespace SlackNet.WebApi;

public class DndResponse : SlackObject
{
    public bool DndEnabled { get; set; }
    public int NextDndStartTs { get; set; }
    [JsonIgnore]
    public DateTime? NextDndStart => NextDndStartTs.ToDateTime();
    public int NextDndEndTs { get; set; }
    [JsonIgnore]
    public DateTime? NextDndEnd => NextDndEndTs.ToDateTime();
}