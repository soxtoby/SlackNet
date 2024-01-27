using System.Text.Json;
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
    private const string InputActionId = "text_input";
    private const string SingleSelectActionId = "single_select";
    private const string MultiSelectActionId = "multi_select";
    private const string DatePickerActionId = "date_picker";
    private const string TimePickerActionId = "time_picker";
    private const string RadioActionId = "radio";
    private const string CheckboxActionId = "checkbox";
    private const string SingleUserActionId = "single_user";
    private const string RichTextActionId = "rich_text_input";
    private const string FileInputActionId = "file_input";
    public const string ModalCallbackId = "modal_demo";

    private readonly ISlackApiClient _slack;
    public ModalViewDemo(ISlackApiClient slack) => _slack = slack;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains(Trigger, StringComparison.OrdinalIgnoreCase) == true)
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
                                BlockId = "input_block",
                                Optional = true,
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
                                Optional = true,
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
                                Optional = true,
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
                                Optional = true,
                                Element = new DatePicker { ActionId = DatePickerActionId }
                            },
                        new InputBlock
                            {
                                Label = "Time",
                                BlockId = "time_block",
                                Optional = true,
                                Element = new TimePicker { ActionId = TimePickerActionId }
                            },
                        new InputBlock
                            {
                                Label = "Radio options",
                                BlockId = "radio_block",
                                Optional = true,
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
                                Optional = true,
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
                                Optional = true,
                                Element = new UserSelectMenu
                                    {
                                        ActionId = SingleUserActionId
                                    }
                            },
                        new InputBlock
                            {
                                Label = "Rich text",
                                BlockId = "rich_text_block",
                                Optional = true,
                                Element = new RichTextInput
                                    {
                                        ActionId = RichTextActionId,
                                        Placeholder = "Enter some rich text"
                                    }
                            },
                        new InputBlock
                            {
                                Label = "File upload",
                                BlockId = "file_input_block",
                                Optional = true,
                                Element = new FileInput
                                    {
                                        ActionId = FileInputActionId,
                                        MaxFiles = 3
                                    }
                            }
                    },
                Submit = "Submit",
                NotifyOnClose = true,
                PrivateMetadata = JsonSerializer.Serialize(new ModalMetadata(request.Channel.Id, request.Channel.Name)) // Holding onto this info for later
            });
    }

    private static IList<Option> ExampleOptions() =>
        [
            new() { Text = "One", Value = "1" },
            new() { Text = "Two", Value = "2" },
            new() { Text = "Three", Value = "3" }
        ];

    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var metadata = JsonSerializer.Deserialize<ModalMetadata>(viewSubmission.View.PrivateMetadata)!;
        Console.WriteLine($"{viewSubmission.User.Name} submitted the demo modal view in the {metadata.ChannelName} channel");

        var state = viewSubmission.View.State;
        var values = new Dictionary<string, string>
            {
                { "Input", state.GetValue<PlainTextInputValue>(InputActionId).Value ?? "none" },
                { "Single-select", state.GetValue<StaticSelectValue>(SingleSelectActionId).SelectedOption?.Text.Text ?? "none" },
                { "Multi-select", string.Join(", ", state.GetValue<StaticMultiSelectValue>(MultiSelectActionId).SelectedOptions.Select(o => o.Text).DefaultIfEmpty("none")) },
                { "Date", state.GetValue<DatePickerValue>(DatePickerActionId).SelectedDate?.ToString("yyyy-MM-dd") ?? "none" },
                { "Time", state.GetValue<TimePickerValue>(TimePickerActionId).SelectedTime?.ToString("hh\\:mm") ?? "none" },
                { "Radio options", state.GetValue<RadioButtonGroupValue>(RadioActionId).SelectedOption?.Text.Text ?? "none" },
                { "Checkbox options", string.Join(", ", state.GetValue<CheckboxGroupValue>(CheckboxActionId).SelectedOptions.Select(o => o.Text).DefaultIfEmpty("none")) },
                { "Single user select", state.GetValue<UserSelectValue>(SingleUserActionId).SelectedUser is string userId ? Link.User(userId).ToString()! : "none" },
                { "Files", string.Join(", ", state.GetValue<FileInputValue>(FileInputActionId).Files.Select(f => f.Name).DefaultIfEmpty("none")) }
            };
        var richText = state.GetValue<RichTextInputValue>(RichTextActionId)?.RichTextValue
            ?? new RichTextBlock { Elements = { new RichTextSection { Elements = { new RichTextText { Text = "none" } } } } };

        await _slack.Chat.PostMessage(new Message
            {
                Channel = metadata.ChannelId,
                Text = $"You entered: {state.GetValue<PlainTextInputValue>(InputActionId).Value}",
                Blocks =
                    {
                        new SectionBlock
                            {
                                Text = new Markdown("You entered:\n"
                                    + string.Join("\n", values.Select(kv => $"*{kv.Key}:* {kv.Value}")))
                            },
                        new RichTextBlock
                            {
                                Elements =
                                    {
                                        new RichTextSection
                                            {
                                                Elements =
                                                    {
                                                        new RichTextText { Text = "Rich text:", Style = { Bold = true } }
                                                    }
                                            }
                                    }
                            },
                        richText
                    }
            });

        return ViewSubmissionResponse.Null;
    }

    public async Task HandleClose(ViewClosed viewClosed)
    {
        var metadata = JsonSerializer.Deserialize<ModalMetadata>(viewClosed.View.PrivateMetadata)!;
        Console.WriteLine($"{viewClosed.User.Name} cancelled the demo modal view in the {metadata.ChannelName} channel");

        await _slack.Chat.PostMessage(new Message
            {
                Channel = metadata.ChannelId,
                Text = "You cancelled the modal"
            });
    }

    record ModalMetadata(string ChannelId, string ChannelName);
}