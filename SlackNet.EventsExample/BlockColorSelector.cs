using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class BlockColorSelector : IBlockActionHandler<ExternalSelectAction>, IBlockOptionProvider
    {
        private readonly ISlackApiClient _slack;
        public static readonly string ActionId = "color_select";

        public BlockColorSelector(ISlackApiClient slack)
        {
            _slack = slack;
        }

        public async Task Handle(ExternalSelectAction select, BlockActionRequest request)
        {
            await _slack.Chat.PostMessage(new Message
            {
                Text = $"Selected color: {select.SelectedOption.Text.Text} ({select.SelectedOption.Value})",
                Channel = request.Channel.Id
            }).ConfigureAwait(false);
        }

        public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request) => new BlockOptionsResponse { Options = GetBlockOptions(request.Value) };

        private static List<Blocks.Option> GetBlockOptions(string search) =>
            FindColors(search)
                .Select(c => new Blocks.Option { Text = c.Name, Value = $"#{c.R:X2}{c.G:X2}{c.B:X2}" })
                .ToList();

        private static IEnumerable<Color> FindColors(string search) =>
            new ColorConverter().GetStandardValues()
                .Cast<Color>()
                .Where(c => c.Name.ToUpperInvariant().Contains(search.ToUpperInvariant()));

        public static IEnumerable<Block> Blocks => new Block[]
            {
                new SectionBlock
                    {
                        Text = "Choose a color",
                        Accessory = new ExternalSelectMenu
                            {
                                ActionId = ActionId,
                                Placeholder = new PlainText("Select a color")
                            }
                    }
            };
    }
}