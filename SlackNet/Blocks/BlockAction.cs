using System;
using Newtonsoft.Json;

namespace SlackNet.Blocks;

public class BlockAction
{
    public string Type { get; set; }
    public string ActionId { get; set; }
    public string BlockId { get; set; }
    public string ActionTs { get; set; }
    [JsonIgnore]
    public DateTime? ActionTimestamp => ActionTs.ToDateTime();
}