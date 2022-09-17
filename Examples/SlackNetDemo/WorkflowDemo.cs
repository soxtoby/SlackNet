using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNetDemo;

/// <summary>
/// A workflow step for sending someone a simple message.
/// To use this demo, follow the steps at https://api.slack.com/workflows/steps,
/// using the value of <see cref="StepCallbackId"/> as the Callback ID when you create the step for your Slack app.
/// </summary>
class WorkflowDemo : IWorkflowStepEditHandler, IViewSubmissionHandler, IEventHandler<WorkflowStepExecute>
{
    public const string StepCallbackId = "test_step";
    public const string ConfigCallback = "test_step_config";
    private const string MessageUserAction = "message_user_action";
    private const string MessageUserInput = "message_user";
    private const string MessageUserOutput = "sent_message_user";
    private const string MessageTextAction = "message_text_action";
    private const string MessageTextInput = "message_text";
    private const string MessageTextOutput = "sent_message_text";

    private readonly ISlackApiClient _slack;
    public WorkflowDemo(ISlackApiClient slack) => _slack = slack;

    /// <summary>
    /// Shows a configuration dialog for editing the workflow step.
    /// </summary>
    public async Task Handle(WorkflowStepEdit workflowStepEdit)
    {
        Console.WriteLine($"{workflowStepEdit.User.Name} opened the configuration dialog for the demo workflow step in the {workflowStepEdit.Channel.Name} channel");
        
        await _slack.Views.Open(workflowStepEdit.TriggerId, new ConfigurationModalViewDefinition
            {
                CallbackId = ConfigCallback,
                Blocks =
                    {
                        new InputBlock
                            {
                                Label = "Person to send message to",
                                Element = new UserSelectMenu
                                    {
                                        ActionId = MessageUserAction,
                                        InitialUser = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(MessageUserInput, out var user) ? user.Value : null
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Message text",
                                Element = new PlainTextInput
                                    {
                                        ActionId = MessageTextAction,
                                        InitialValue = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(MessageTextInput, out var input) ? input.Value : string.Empty
                                    }
                            }
                    }
            });
    }

    /// <summary>
    /// Updates the workflow step based on the configuration dialog's inputs.
    /// </summary>
    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        Console.WriteLine($"{viewSubmission.User.Name} submitted the configuration dialog for the demo workflow step in the {viewSubmission.Channel.Name} channel");
        
        await _slack.Workflows.UpdateStep(
            viewSubmission.WorkflowStep.WorkflowStepEditId,
            new Dictionary<string, WorkflowInput>
                {
                    { MessageUserInput, new WorkflowInput { Value = viewSubmission.View.State.GetValue<UserSelectValue>(MessageUserAction).SelectedUser } },
                    { MessageTextInput, new WorkflowInput { Value = viewSubmission.View.State.GetValue<PlainTextInputValue>(MessageTextAction).Value } }
                },
            new List<WorkflowOutput>
                {
                    new() { Label = "Test Recipient", Name = MessageUserOutput, Type = WorkflowOutputType.User },
                    new() { Label = "Test Message", Name = MessageTextOutput, Type = WorkflowOutputType.Text }
                });
        return ViewSubmissionResponse.Null;
    }

    public async Task HandleClose(ViewClosed viewClosed)
    {
        Console.WriteLine($"{viewClosed.User.Name} cancelled the configuration dialog for the demo workflow step in the {viewClosed.Channel.Name} channel");
    }

    /// <summary>
    /// Sends the configured message to the configured user.
    /// </summary>
    public async Task Handle(WorkflowStepExecute slackEvent)
    {
        var user = slackEvent.WorkflowStep.Inputs[MessageUserInput].Value;
        var message = slackEvent.WorkflowStep.Inputs[MessageTextInput].Value;
        
        Console.WriteLine($"Demo workflow step is sending a message to {(await _slack.Users.Info(user)).Name}");

        await _slack.Chat.PostMessage(new Message
            {
                Channel = user,
                Text = message
            });

        await _slack.Workflows.StepCompleted(slackEvent.WorkflowStep.WorkflowStepExecuteId, new Dictionary<string, string>
            {
                { MessageUserOutput, user },
                { MessageTextOutput, message }
            });
    }
}