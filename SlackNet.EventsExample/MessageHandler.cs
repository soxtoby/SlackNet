using System.Threading.Tasks;
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
            if (message.Text.Contains("test interactivity"))
                await _slack.Chat.PostMessage(new Message
                    {
                        Text = "Here's some interactivity for you",
                        Channel = message.Channel,
                        Attachments =
                            {
                                new Attachment
                                    {
                                        Text = "Counter: 0",
                                        CallbackId = "counter",
                                        Actions = Counter.Actions
                                    },
                                new Attachment
                                    {
                                        Text = "Choose a color",
                                        Fallback = "No color for you",
                                        CallbackId = "color",
                                        Actions = ColorSelector.Actions
                                    }
                            }
                    }).ConfigureAwait(false);
        }
    }
}