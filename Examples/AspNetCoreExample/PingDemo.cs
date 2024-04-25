using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AspNetCoreExample;

/// <summary>
/// A simple event handler that says pong when you say ping.
/// </summary>
class PingDemo(ISlackApiClient slack, ILogger<PingDemo> log) : IEventHandler<MessageEvent>
{
    private readonly ILogger _log = log;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains("ping", StringComparison.OrdinalIgnoreCase) == true)
        {
            _log.LogInformation("Received ping from {User} in the {Channel} channel", (await slack.Users.Info(slackEvent.User)).Name, (await slack.Conversations.Info(slackEvent.Channel)).Name);
            
            await slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                });
        }
    }
}