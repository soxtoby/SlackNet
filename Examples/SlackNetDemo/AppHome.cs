using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;

namespace SlackNetDemo;

/// <summary>
/// Displays a list of things you can do with this demo when you open the app's home screen. 
/// </summary>
class AppHome(ISlackApiClient slack) : IEventHandler<AppHomeOpened>
{
    public async Task Handle(AppHomeOpened slackEvent)
    {
        if (slackEvent.Tab == AppHomeTab.Home)
        {
            Console.WriteLine($"{(await slack.Users.Info(slackEvent.User)).Name} opened the app's home view");
            
            await slack.Views.Publish(slackEvent.User, new HomeViewDefinition
                {
                    Blocks =
                        {
                            new SectionBlock
                                {
                                    Text = new Markdown($"""
                                        Welcome to the SlackNet example. Here's what you can do:
                                        • Say "{PingDemo.Trigger}" to get back a pong
                                        • Say "{CounterDemo.Trigger}" to get the counter demo
                                        • Say "{ModalViewDemo.Trigger}" to open then modal view demo
                                        • Use the `{EchoDemo.SlashCommand}` slash command to see an echo
                                        """)
                                }
                        }
                }, slackEvent.View?.Hash);
        }
    }
}