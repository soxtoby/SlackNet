using System.Linq;
using System.Threading.Tasks;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.EventsExample
{
    public class MessageHandler : IEventHandler<MessageEvent>
    {
        private readonly ISlackApiClient _slack;

        public MessageHandler(ISlackApiClient slack) => _slack = slack;

        public async Task Handle(MessageEvent message)
        {
            if (message.Text?.Contains("test interactivity") ?? false)
                await _slack.Chat.PostMessage(new Message
                    {
                        Text = "No interactivity for you",
                        Channel = message.Channel,
                        Blocks = new Block[] { new SectionBlock { Text = "Here's some interactivity for you" } }
                            .Concat(BlockCounter.Blocks)
                            .Concat(new[] { new DividerBlock() })
                            .Concat(BlockColorSelector.Blocks)
                            .Concat(new[] { new DividerBlock() })
                            .Concat(BlockDialogDemo.Blocks)
                            .ToList(),
                        Attachments =
                            {
                                new Attachment
                                    {
                                        Text = "Counter: 0",
                                        Fallback = "No counter for you",
                                        CallbackId = "counter",
                                        Actions = LegacyCounter.Actions
                                    },
                                new Attachment
                                    {
                                        Text = "Choose a color",
                                        Fallback = "No color for you",
                                        CallbackId = "color",
                                        Actions = LegacyColorSelector.Actions
                                    },
                                new Attachment
                                    {
                                        Text = "Dialogs",
                                        Fallback = "No dialogs for you",
                                        CallbackId = "dialog",
                                        Actions = LegacyDialogDemo.Actions
                                    }
                            }
                    }).ConfigureAwait(false);
        }
    }
}