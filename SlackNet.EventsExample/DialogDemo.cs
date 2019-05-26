using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class DialogDemo : IDialogSubmissionHandler, IInteractiveMessageHandler
    {
        internal const string EchoDialog = "echo-dialog";
        internal const string ErrorDialog = "error-dialog";

        private readonly ISlackApiClient _slack;
        public DialogDemo(ISlackApiClient slack) => _slack = slack;

        public async Task<MessageResponse> Handle(InteractiveMessage message)
        {
            string title;
            string submitLabel;

            if (message.Action.Name == EchoDialog)
            {
                title = "Echo Dialog";
                submitLabel = "Echo";
            }
            else
            {
                title = "Error Dialog";
                submitLabel = "Fail";
            }

            await _slack.Dialog.Open(message.TriggerId, new Dialog
                {
                    Title = title,
                    SubmitLabel = submitLabel,
                    CallbackId = message.Action.Name,
                    Elements =
                        {
                            new TextElement
                                {
                                    Name = "text",
                                    Label = "Text",
                                    Placeholder = "Enter some text"
                                },
                            new TextArea
                                {
                                    Name = "textarea",
                                    Label = "Text Area",
                                    Placeholder = "Enter some more text"
                                },
                            new SelectElement
                                {
                                    Name = "select",
                                    Label = "Select",
                                    Placeholder = "Select an option",
                                    Options =
                                        {
                                            new SelectOption { Label = "Cats", Value = "felines" },
                                            new SelectOption { Label = "Dogs", Value = "canines" }
                                        }
                                }
                        }

                }).ConfigureAwait(false);

            return null;
        }

        public async Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog)
        {
            if (dialog.CallbackId == EchoDialog)
            {
                await _slack.Chat.PostMessage(new Message
                    {
                        Channel = dialog.Channel.Id,
                        Attachments =
                            {
                                new Attachment
                                    {
                                        Text = "Dialog fields",
                                        Fields = dialog.Submission
                                            .Select(e => new Field
                                                {
                                                    Title = e.Key,
                                                    Value = e.Value
                                                })
                                            .ToList()
                                    }
                            }
                    }).ConfigureAwait(false);
                return null;
            }
            else
            {
                return dialog.Submission.Keys
                    .Select(name => new DialogError { Name = name, Error = $"Not {name}-y enough!" });
            }
        }

        public Task HandleCancel(DialogCancellation cancellation)
        {
            return Task.CompletedTask;
        }

        public static IList<ActionElement> Actions => new List<ActionElement>
            {
                new Button
                    {
                        Text = "Echo dialog",
                        Name = EchoDialog
                    },
                new Button
                    {
                        Text = "Error dialog",
                        Name = ErrorDialog
                    }
            };
    }
}