using Microsoft.Extensions.Logging;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace AzureFunctionsExample;

/// <summary>
/// A simple event handler that says pong when you say ping.
/// </summary>
class PingDemo(
    ISlackApiClient slack,
    ILogger<PingDemo> logger
) : IEventHandler<MessageEvent>
{
    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains("ping", StringComparison.OrdinalIgnoreCase) == true)
        {
            logger.LogInformation("Received ping from {User} in the {Channel} channel", (await slack.Users.Info(slackEvent.User)).Name, (await slack.Conversations.Info(slackEvent.Channel)).Name);

            await slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                });
        }
    }
}