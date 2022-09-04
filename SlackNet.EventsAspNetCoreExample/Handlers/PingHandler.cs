using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNet.EventsAspNetCoreExample.Handlers
{
    public class PingHandler : IEventHandler<MessageEvent>
    {

        private readonly ISlackApiClient _slack;
        public PingHandler(ISlackApiClient slack)
        {
            _slack = slack;
        }

        public async Task Handle(MessageEvent slackEvent)
        {
            if (slackEvent.Text.Contains("ping"))
            {
                await _slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                }).ConfigureAwait(false);
            }
        }
    }
}