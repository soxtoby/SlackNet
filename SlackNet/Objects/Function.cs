using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// Metadata for a function used as a workflow step.
/// </summary>
public class Function
{
    public string Id { get; set; } = null!;
    public string? CallbackId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public IList<FunctionParameter> InputParameters { get; set; } = [];
    public IList<FunctionParameter> OutputParameters { get; set; } = [];
    public string? AppId { get; set; }
    public int DateCreated { get; set; }
    [JsonIgnore]
    public DateTime Created => DateCreated.ToDateTime().GetValueOrDefault();
    public int DateUpdated { get; set; }
    [JsonIgnore]
    public DateTime? Updated => DateUpdated.ToDateTime();
    public int DateDeleted { get; set; }
    [JsonIgnore]
    public DateTime? Deleted => DateDeleted.ToDateTime();
}
