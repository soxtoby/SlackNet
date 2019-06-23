using System.Collections.Generic;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Button = SlackNet.Blocks.Button;

namespace SlackNet.EventsExample
{
    public class BlockDialogDemo : DialogDemoBase, IBlockActionHandler<ButtonAction>
    {
        public BlockDialogDemo(ISlackApiClient slack) : base(slack) { }

        public async Task Handle(ButtonAction action, BlockActionRequest request)
        {
            await OpenDialog(action.ActionId, request.TriggerId).ConfigureAwait(false);
        }

        public static IEnumerable<Block> Blocks => new Block[]
            {
                new SectionBlock { Text = "Dialogs" },
                new ActionsBlock
                    {
                        Elements =
                            {
                                new Button
                                    {
                                        Text = "Echo dialog",
                                        ActionId = EchoDialog
                                    },
                                new Button
                                    {
                                        Text = "Error dialog",
                                        ActionId = ErrorDialog
                                    }
                            }
                    }
            };
    }
}