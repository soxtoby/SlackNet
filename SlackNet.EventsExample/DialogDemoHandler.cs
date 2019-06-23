using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class DialogDemoHandler  : IDialogSubmissionHandler
    {
        private readonly ISlackApiClient _slack;

        public DialogDemoHandler(ISlackApiClient slack)
        {
            _slack = slack;
        }

        public async Task<IEnumerable<DialogError>> Handle(DialogSubmission dialog)
        {
            if (dialog.CallbackId == DialogDemoBase.EchoDialog)
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
    }
}