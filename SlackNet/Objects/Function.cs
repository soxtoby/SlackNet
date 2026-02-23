using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// Metadata for a function used as a workflow step.
/// </summary>
public class Function
{
    public string Id { get; set; }
    public string CallbackId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public IList<FunctionParameter> InputParameters { get; set; } = [];
    public IList<FunctionParameter> OutputParameters { get; set; } = [];
    public string AppId { get; set; }
    public int DateCreated { get; set; }
    public int DateUpdated { get; set; }
    public int DateDeleted { get; set; }
}
