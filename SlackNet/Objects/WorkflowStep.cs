using System.Collections.Generic;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet;

public class WorkflowStep
{
    /// <summary>
    /// An identifier provided with <see cref="WorkflowStepExecute"/> event payloads. Used to call back to <see cref="IWorkflowsApi.StepCompleted"/> or <see cref="IWorkflowsApi.StepFailed"/>.
    /// </summary>
    public string WorkflowStepExecuteId { get; set; }

    /// <summary>
    /// An identifier provided with <see cref="WorkflowStepExecute"/> payloads. Unique to the current execution of the workflow.
    /// </summary>
    public string WorkflowInstanceId { get; set; }

    /// <summary>
    /// An identifier for the workflow that the step is included in. Consistent across workflow executions.
    /// </summary>
    public string WorkflowId { get; set; }

    /// <summary>
    /// An identifier for the step within the workflow. Consistent across workflow executions.
    /// </summary>
    public string StepId { get; set; }

    /// <summary>
    /// A key-value map of <see cref="WorkflowInput"/> objects that specify input required from a user.
    /// This is the data your app expects to receive when the workflow step starts.
    /// </summary>
    public IDictionary<string, WorkflowInput> Inputs { get; set; } = new Dictionary<string, WorkflowInput>();

    /// <summary>
    /// A list of <see cref="WorkflowOutput"/> objects created during step execution.
    /// This is the data your app promises to provide when your workflow step is executed.
    /// </summary>
    public IList<WorkflowOutput> Outputs { get; set; } = new List<WorkflowOutput>();
}