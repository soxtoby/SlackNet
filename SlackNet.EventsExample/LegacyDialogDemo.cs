using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Interaction;
using ActionElement = SlackNet.Interaction.ActionElement;
using Button = SlackNet.Interaction.Button;

namespace SlackNet.EventsExample
{
    public class LegacyDialogDemo : DialogDemoBase, IInteractiveMessageHandler
    {
        public LegacyDialogDemo(ISlackApiClient slack) : base(slack) { }

        public async Task<MessageResponse> Handle(InteractiveMessage message)
        {
            await OpenDialog(message.Action.Name, message.TriggerId).ConfigureAwait(false);

            return null;
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