using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

public class FileComment
{
    public string Id { get; set; }
    public int Created { get; set; }
    [JsonIgnore]
    public DateTime CreatedDate => Created.ToDateTime().GetValueOrDefault();
    public int Timestamp { get; set; }
    public string User { get; set; }
    public string Comment { get; set; }
    public string Channel { get; set; }
    public IList<Reaction> Reactions { get; set; }
}