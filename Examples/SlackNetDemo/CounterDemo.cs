using System.Text.RegularExpressions;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace SlackNetDemo;

/// <summary>
/// Displays an interactive message that updates itself.
/// </summary>
class CounterDemo : IEventHandler<MessageEvent>, IBlockActionHandler<ButtonAction>
{
    private const string ActionPrefix = "add";
    public const string Add1 = ActionPrefix + "1";
    public const string Add5 = ActionPrefix + "5";
    public const string Add10 = ActionPrefix + "10";
    public const string Trigger = "counter demo";
    private static readonly Regex CounterPattern = new("Counter: (\\d+)");

    private readonly ISlackApiClient _slack;
    public CounterDemo(ISlackApiClient slack) => _slack = slack;

    public async Task Handle(MessageEvent slackEvent)
    {
        if (slackEvent.Text.Contains(Trigger, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"{(await _slack.Users.Info(slackEvent.User)).Name} asked for a counter demo in the {(await _slack.Conversations.Info(slackEvent.Channel)).Name} channel");
            
            await _slack.Chat.PostMessage(new Message
                {
                    Channel = slackEvent.Channel,
                    Blocks = Blocks
                });
        }
    }

    public async Task Handle(ButtonAction button, BlockActionRequest request)
    {
        Console.WriteLine($"{request.User.Name} clicked on the Add {button.Value} button in the {request.Channel.Name} channel");
        
        var counter = SectionBeforeAddButtons(button, request);
        if (counter != null)
        {
            var counterText = CounterPattern.Match(counter.Text.Text ?? string.Empty);
            if (counterText.Success)
            {
                var count = int.Parse(counterText.Groups[1].Value);
                var increment = int.Parse(((ButtonAction)request.Action).Value);
                counter.Text = $"Counter: {count + increment}";
                await _slack.Chat.Update(new MessageUpdate
                    {
                        Ts = request.Message.Ts,
                        Text = request.Message.Text,
                        Blocks = request.Message.Blocks,
                        ChannelId = request.Channel.Id
                    });
            }
        }
    }

    private static SectionBlock? SectionBeforeAddButtons(ButtonAction button, BlockActionRequest request)
    {
        return request.Message.Blocks
            .TakeWhile(b => b.BlockId != button.BlockId)
            .LastOrDefault() as SectionBlock;
    }

    static List<Block> Blocks => new()
        {
            new SectionBlock { Text = "Counter: 0" },
            new ActionsBlock
                {
                    Elements =
                        {
                            new SlackNet.Blocks.Button
                                {
                                    ActionId = Add1,
                                    Value = "1",
                                    Text = new PlainText("Add 1")
                                },
                            new SlackNet.Blocks.Button
                                {
                                    ActionId = Add5,
                                    Value = "5",
                                    Text = new PlainText("Add 5")
                                },
                            new SlackNet.Blocks.Button
                                {
                                    ActionId = Add10,
                                    Value = "10",
                                    Text = new PlainText("Add 10")
                                }
                        }
                }
        };
}