using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

public class FileComment
{
    public string Id { get; set; } = null!;
    public int Created { get; set; }
    [JsonIgnore]
    public DateTime CreatedDate => Created.ToDateTime().GetValueOrDefault();
    public int Timestamp { get; set; }
    public string User { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string? Channel { get; set; }
    public IList<Reaction>? Reactions { get; set; }
}