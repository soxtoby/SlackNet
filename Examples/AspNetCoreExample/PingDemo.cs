using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AspNetCoreExample;

/// <summary>
/// A simple event handler that says pong when you say ping.
/// </summary>
class PingDemo : IEventHandler<MessageEvent>
{
    private readonly ISlackApiClient _slack;
    private readonly ILogger _log;

    public PingDemo(ISlackApiClient slack, ILogger log)
    {
        _slack = slack;
        _log = log;
    }

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text?.Contains("ping", StringComparison.OrdinalIgnoreCase) == true)
        {
            _log.LogInformation("Received ping from {User} in the {Channel} channel", (await _slack.Users.Info(slackEvent.User)).Name, (await _slack.Conversations.Info(slackEvent.Channel)).Name);
            
            await _slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                });
        }
    }
}