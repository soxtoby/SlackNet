using System.Collections.Generic;

namespace SlackNet.WebApi;

public class WorkflowStepsVersionsResponse
{
    public IList<WorkflowStepsVersion> StepsVersions { get; set; } = [];
}

public class WorkflowStepsVersion
{
    public string Title { get; set; }
    public string WorkflowId { get; set; }
    public string StepId { get; set; }
    public bool IsDeleted { get; set; }
    public string WorkflowVersionCreated { get; set; }
}