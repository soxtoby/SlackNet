using System.Threading.Tasks;
using SlackNet.Interaction;

namespace SlackNet.EventsExample
{
    public class DialogDemoBase
    {
        internal const string EchoDialog = "echo-dialog";
        internal const string ErrorDialog = "error-dialog";

        private readonly ISlackApiClient _slack;

        public DialogDemoBase(ISlackApiClient slack)
        {
            _slack = slack;
        }

        protected async Task OpenDialog(string action, string triggerId)
        {
            string title;
            string submitLabel;

            if (action == EchoDialog)
            {
                title = "Echo Dialog";
                submitLabel = "Echo";
            }
            else
            {
                title = "Error Dialog";
                submitLabel = "Fail";
            }

            await _slack.Dialog.Open(triggerId, new Dialog
                {
                    Title = title,
                    SubmitLabel = submitLabel,
                    CallbackId = action,
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
        }
    }
}