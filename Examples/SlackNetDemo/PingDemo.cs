using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace SlackNetDemo;

/// <summary>
/// A simple event handler that says pong when you say ping.
/// </summary>
class PingDemo : IEventHandler<MessageEvent>
{
    public const string Trigger = "ping";

    private readonly ISlackApiClient _slack;
    public PingDemo(ISlackApiClient slack) => _slack = slack;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text.Contains(Trigger, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Received ping from {(await _slack.Users.Info(slackEvent.User)).Name} in the {(await _slack.Conversations.Info(slackEvent.Channel)).Name} channel");
            
            await _slack.Chat.PostMessage(new Message
                {
                    Text = "pong",
                    Channel = slackEvent.Channel
                });
        }
    }
}