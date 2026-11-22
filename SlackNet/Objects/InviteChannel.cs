using System;
using Newtonsoft.Json;

namespace SlackNet;

public class InviteChannel
{
    public string Id { get; set; } = null!;
    public bool IsPrivate { get; set; }
    public bool IsIm { get; set; }
    public string? Name { get; set; }
    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
    public long MessageLatestCountedTimestamp { get; set; }
}