using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class BlockCounter : IBlockActionHandler<ButtonAction>
    {
        private readonly ISlackApiClient _slack;
        private static readonly string ActionPrefix = "add";
        public static readonly string Add1 = ActionPrefix + "1";
        public static readonly string Add5 = ActionPrefix + "5";
        public static readonly string Add10 = ActionPrefix + "10";

        private static readonly Regex _counterPattern = new Regex("Counter: (\\d+)");

        public BlockCounter(ISlackApiClient slack)
        {
            _slack = slack;
        }

        public async Task Handle(ButtonAction button, BlockActionRequest request)
        {
            var counter = SectionBeforeAddButtons(button, request);
            if (counter != null)
            {
                var counterText = _counterPattern.Match(counter.Text.Text ?? string.Empty);
                if (counterText.Success)
                {
                    var count = int.Parse(counterText.Groups[1].Value);
                    var increment = int.Parse(((ButtonAction)request.Action).Value);
                    counter.Text = $"Counter: {count + increment}";
                    await _slack.Chat.Update(new MessageUpdate
                    {
                        Ts = request.Message.Ts,
                        Text = request.Message.Text,
                        Blocks = request.Message.Blocks,
                        Attachments = request.Message.Attachments,
                        ChannelId = request.Channel.Id
                    }).ConfigureAwait(false);
                }
            }
        }

        private static SectionBlock SectionBeforeAddButtons(ButtonAction button, BlockActionRequest request)
        {
            return request.Message.Blocks
                .TakeWhile(b => b.BlockId != button.BlockId)
                .LastOrDefault() as SectionBlock;
        }

        public static IEnumerable<Block> Blocks => new Block[]
            {
                new SectionBlock { Text = "Counter: 0" },
                new ActionsBlock
                    {
                        Elements =
                            {
                                new Blocks.Button
                                    {
                                        ActionId = Add1,
                                        Value = "1",
                                        Text = new PlainText("Add 1")
                                    },
                                new Blocks.Button
                                    {
                                        ActionId = Add5,
                                        Value = "5",
                                        Text = new PlainText("Add 5")
                                    },
                                new Blocks.Button
                                    {
                                        ActionId = Add10,
                                        Value = "10",
                                        Text = new PlainText("Add 10")
                                    }
                            }
                    }
            };
    }
}