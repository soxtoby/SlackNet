using System;
using Newtonsoft.Json;

namespace SlackNet;

public class OwnDndStatus : DndStatus
{
    public bool SnoozeEnabled { get; set; }
    public int SnoozeEndtime { get; set; }
    [JsonIgnore]
    public DateTime? SnoozeEnd => SnoozeEndtime.ToDateTime();
}