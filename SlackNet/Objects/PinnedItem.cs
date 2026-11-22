using System;
using Newtonsoft.Json;

namespace SlackNet;

public class PinnedItem
{
    public string Type { get; set; } = null!;
    public int Created { get; set; }
    [JsonIgnore]
    public DateTime CreatedTimestamp => Created.ToDateTime().GetValueOrDefault();
    public string CreatedBy { get; set; } = null!;
}