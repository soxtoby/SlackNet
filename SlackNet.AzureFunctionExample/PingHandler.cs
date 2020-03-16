using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.AzureFunctionExample
{
    public class PingHandler : IEventHandler<MessageEvent>
    {
        private static int _count;

        private readonly ISlackApiClient _slack;
        public PingHandler(ISlackApiClient slack) => _slack = slack;

        public async Task Handle(MessageEvent slackEvent)
        {
            if (slackEvent.Text.Contains("ping"))
                await _slack.Chat.PostMessage(new Message
                    {
                        Text = "pong",
                        Attachments = { new Attachment { Text = $"Count: {++_count}" } },
                        Channel = slackEvent.Channel
                    }).ConfigureAwait(false);
        }
    }
}