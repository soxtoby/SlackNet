namespace SlackNet.Events
{
    /// <summary>
    /// A workflow step supported by your app should execute
    /// </summary>
    public class WorkflowStepExecute : Event
    {
        public string CallbackId { get; set; }

        public WorkflowStep WorkflowStep { get; set; }
    }
}