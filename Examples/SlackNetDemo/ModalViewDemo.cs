using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;
using Button = SlackNet.Blocks.Button;
using Option = SlackNet.Blocks.Option;

namespace SlackNetDemo;

/// <summary>
/// Opens a modal view with a range of different inputs.
/// </summary>
class ModalViewDemo : IEventHandler<MessageEvent>, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    public const string Trigger = "modal demo";
    public const string OpenModal = "open_modal";
    private const string InputBlockId = "input_block";
    private const string InputActionId = "text_input";
    private const string SingleSelectActionId = "single_select";
    private const string MultiSelectActionId = "multi_select";
    private const string DatePickerActionId = "date_picker";
    private const string TimePickerActionId = "time_picker";
    private const string RadioActionId = "radio";
    private const string CheckboxActionId = "checkbox";
    private const string SingleUserActionId = "single_user";
    public const string ModalCallbackId = "modal_demo";

    private readonly ISlackApiClient _slack;
    public ModalViewDemo(ISlackApiClient slack) => _slack = slack;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text.Contains(Trigger, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"{(await _slack.Users.Info(slackEvent.User)).Name} asked for a modal view demo in the {(await _slack.Conversations.Info(slackEvent.Channel)).Name} channel");
            
            await _slack.Chat.PostMessage(new Message
                {
                    Channel = slackEvent.Channel,
                    Blocks =
                        {
                            new SectionBlock { Text = "Here's the modal view demo" },
                            new ActionsBlock
                                {
                                    Elements =
                                        {
                                            new Button
                                                {
                                                    ActionId = OpenModal,
                                                    Text = "Open modal"
                                                }
                                        }
                                }
                        }
                });
        }
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        Console.WriteLine($"{request.User.Name} clicked the Open modal button in the {request.Channel.Name} channel");
        
        await _slack.Views.Open(request.TriggerId, new ModalViewDefinition
            {
                Title = "Example Modal",
                CallbackId = ModalCallbackId,
                Blocks =
                    {
                        new InputBlock
                            {
                                Label = "Input",
                                BlockId = InputBlockId,
                                Element = new PlainTextInput
                                    {
                                        ActionId = InputActionId,
                                        Placeholder = "Enter some text"
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Single-select",
                                BlockId = "single_select_block",
                                Element = new StaticSelectMenu
                                    {
                                        ActionId = SingleSelectActionId,
                                        Options = ExampleOptions()
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Multi-select",
                                BlockId = "multi_select_block",
                                Element = new StaticMultiSelectMenu
                                    {
                                        ActionId = MultiSelectActionId,
                                        Options = ExampleOptions()
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Date",
                                BlockId = "date_block",
                                Element = new DatePicker { ActionId = DatePickerActionId }
                            },
                        new InputBlock
                            {
                                Label = "Time",
                                BlockId = "time_block",
                                Element = new TimePicker { ActionId = TimePickerActionId }
                            },
                        new InputBlock
                            {
                                Label = "Radio options",
                                BlockId = "radio_block",
                                Element = new RadioButtonGroup
                                    {
                                        ActionId = RadioActionId,
                                        Options = ExampleOptions()
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Checkbox options",
                                BlockId = "checkbox_block",
                                Element = new CheckboxGroup
                                    {
                                        ActionId = CheckboxActionId,
                                        Options = ExampleOptions()
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Single user select",
                                BlockId = "single_user_block",
                                Element = new UserSelectMenu
                                    {
                                        ActionId = SingleUserActionId
                                    }
                            }
                    },
                Submit = "Submit",
                NotifyOnClose = true
            });
    }

    private static IList<Option> ExampleOptions() => new List<Option>
        {
            new() { Text = "One", Value = "1" },
            new() { Text = "Two", Value = "2" },
            new() { Text = "Three", Value = "3" }
        };

    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        Console.WriteLine($"{viewSubmission.User.Name} submitted the demo modal view in the {viewSubmission.Channel.Name} channel");
        
        var state = viewSubmission.View.State;
        var values = new Dictionary<string, string>
            {
                { "Input", state.GetValue<PlainTextInputValue>(InputActionId).Value },
                { "Single-select", state.GetValue<StaticSelectValue>(SingleSelectActionId).SelectedOption?.Text.Text ?? "none" },
                { "Multi-select", string.Join(", ", state.GetValue<StaticMultiSelectValue>(MultiSelectActionId).SelectedOptions.Select(o => o.Text).DefaultIfEmpty("none")) },
                { "Date", state.GetValue<DatePickerValue>(DatePickerActionId).SelectedDate?.ToString("yyyy-MM-dd") ?? "none" },
                { "Time", state.GetValue<TimePickerValue>(TimePickerActionId).SelectedTime?.ToString("hh\\:mm") ?? "none" },
                { "Radio options", state.GetValue<RadioButtonGroupValue>(RadioActionId).SelectedOption?.Text.Text ?? "none" },
                { "Checkbox options", string.Join(", ", state.GetValue<CheckboxGroupValue>(CheckboxActionId).SelectedOptions.Select(o => o.Text).DefaultIfEmpty("none")) },
                { "Single user select", state.GetValue<UserSelectValue>(SingleUserActionId).SelectedUser ?? "none" }
            };

        await _slack.Chat.PostMessage(new Message
            {
                Channel = viewSubmission.Channel.Id,
                Text = $"You entered: {state.GetValue<PlainTextInputValue>(InputActionId).Value}",
                Blocks =
                    {
                        new SectionBlock
                            {
                                Text = new Markdown("You entered:\n"
                                    + string.Join("\n", values.Select(kv => $"*{kv.Key}:* {kv.Value}")))
                            }
                    }
            });

        return ViewSubmissionResponse.Null;
    }

    public async Task HandleClose(ViewClosed viewClosed)
    {
        Console.WriteLine($"{viewClosed.User.Name} cancelled the demo modal view in the {viewClosed.Channel.Name} channel");
        
        await _slack.Chat.PostMessage(new Message
            {
                Channel = viewClosed.Channel.Id,
                Text = "You cancelled the modal"
            });
    }
}