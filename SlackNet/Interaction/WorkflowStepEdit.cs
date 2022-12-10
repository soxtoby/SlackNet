namespace SlackNet.Interaction;

public class WorkflowStepEdit : InteractionRequest
{
    public string TriggerId { get; set; }
    public string CallbackId { get; set; }

    public WorkflowStep WorkflowStep { get; set; }
}