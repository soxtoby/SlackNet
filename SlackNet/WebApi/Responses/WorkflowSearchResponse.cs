#nullable enable
using System.Collections.Generic;

namespace SlackNet.WebApi;

public class WorkflowSearchResponse
{
    public IList<Workflow> Workflows { get; set; } = [];

    /// <summary>
    /// The total number of workflows that match this search criteria.
    /// </summary>
    public int TotalFound { get; set; }

    public ResponseMetadata ResponseMetadata { get; set; } = new();
}
