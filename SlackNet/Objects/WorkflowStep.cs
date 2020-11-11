using System.Collections.Generic;

namespace SlackNet
{
    public class WorkflowStep
    {

        /// <summary>
        /// An identifier provided with workflow_step_execute event payloads. Used to call back to workflows.stepCompleted or workflows.stepFailed.
        /// </summary>
        public string WorkflowStepExecuteId { get; set; }

        /// <summary>
        /// An identifier provided with view_submission payloads. Used to call back to workflows.updateStep
        /// </summary>
        public string WorkflowStepEditId { get; set; }

        /// <summary>
        /// An identifier provided with workflow_step_execute payloads. Unique to the current execution of the workflow.
        /// </summary>
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// An identifier for the workflow that the step is included in. Consistent across workflow executions.
        /// </summary>
        public string  WorkflowId { get; set; }

        /// <summary>
        /// An identifier for the step within the workflow. Consistent across workflow executions.
        /// </summary>
        public string StepId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public IDictionary<string, WorkflowInput> Inputs { get; set; }

        /// <summary>
        /// A key-value map of <see cref="WorkflowInput"/> objects that specify input required from a user. This is the data your app expects to receive when the workflow step starts.
        /// </summary>
        public WorkflowOutput[] Outputs { get; set; }
    }
}