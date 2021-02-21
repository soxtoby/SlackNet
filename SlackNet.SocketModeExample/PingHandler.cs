using System;
using System.Threading.Tasks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.SocketModeExample
{
    public class PingHandler : IEventHandler<MessageEvent>
    {
        private static int _count;

        private readonly ISlackApiClient _slack;
        public PingHandler(ISlackApiClient slack) => _slack = slack;

        public async Task Handle(MessageEvent slackEvent)
        {
            if (slackEvent.Text.Contains("ping"))
            {
                var user = await _slack.Users.Info(slackEvent.User).ConfigureAwait(false);
                var channel = await _slack.Conversations.Info(slackEvent.Channel).ConfigureAwait(false);

                Console.WriteLine($"Received ping from @{user.Name} in #{channel.Name}");

                await _slack.Chat.PostMessage(new Message
                    {
                        Text = "pong",
                        Attachments = { new Attachment { Text = $"Count: {++_count}" } },
                        Channel = slackEvent.Channel
                    }).ConfigureAwait(false);
            }
        }
    }
}