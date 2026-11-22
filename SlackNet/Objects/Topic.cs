using System;
using Newtonsoft.Json;

namespace SlackNet;

public class Topic
{
    public string? Value { get; set; }
    public string? Creator { get; set; }
    public int LastSet { get; set; }
    [JsonIgnore]
    public DateTime? LastSetDate => LastSet.ToDateTime();
}