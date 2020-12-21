using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    /// <summary>
    /// To use this example, follow the steps at https://api.slack.com/workflows/steps,
    /// using the value of <see cref="StepCallbackId"/> as the Callback ID when you create the step for your Slack app.
    /// </summary>
    public class WorkflowExample : IWorkflowStepEditHandler, IViewSubmissionHandler, IEventHandler<WorkflowStepExecute>
    {
        private readonly ISlackApiClient _slack;
        internal const string StepCallbackId = "test_step";
        internal const string ConfigCallback = "test_step_config";
        private const string MessageUserAction = "message_user_action";
        private const string MessageUserInput = "message_user";
        private const string MessageUserOutput = "sent_message_user";
        private const string MessageTextAction = "message_text_action";
        private const string MessageTextInput = "message_text";
        private const string MessageTextOutput = "sent_message_text";

        public WorkflowExample(ISlackApiClient slack)
        {
            _slack = slack;
        }

        public async Task Handle(WorkflowStepEdit workflowStepEdit)
        {
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
                                            InitialValue = workflowStepEdit.WorkflowStep.Inputs.TryGetValue(MessageTextInput, out var input) ? input.Value as string : string.Empty
                                        }
                                }
                        }
                });
        }

        public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
        {
            await _slack.Workflows.UpdateStep(
                viewSubmission.WorkflowStep.WorkflowStepEditId,
                new Dictionary<string, WorkflowInput>
                    {
                        { MessageUserInput, new WorkflowInput { Value = viewSubmission.View.State.GetValue<UserSelectValue>(MessageUserAction).SelectedUser } },
                        { MessageTextInput, new WorkflowInput { Value = viewSubmission.View.State.GetValue<PlainTextInputValue>(MessageTextAction).Value } }
                    },
                new List<WorkflowOutput>
                    {
                        new WorkflowOutput { Label = "Test Recipient", Name = MessageUserOutput, Type = WorkflowOutputType.User },
                        new WorkflowOutput { Label = "Test Message", Name = MessageTextOutput, Type = WorkflowOutputType.Text }
                    });
            return ViewSubmissionResponse.Null;
        }

        public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;

        public async Task Handle(WorkflowStepExecute slackEvent)
        {
            var user = slackEvent.WorkflowStep.Inputs[MessageUserInput].Value;
            var message = slackEvent.WorkflowStep.Inputs[MessageTextInput].Value;

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
}