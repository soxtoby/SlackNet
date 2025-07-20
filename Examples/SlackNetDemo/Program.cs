using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Extensions.DependencyInjection;
using SlackNetDemo;

Console.WriteLine("Configuring...");

var settings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build()
    .Get<AppSettings>();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(settings.ApiToken) // This gets used by the API client
    .UseAppLevelToken(settings.AppLevelToken) // This gets used by the socket mode client

    // Start with the app home screen - it lets you know what you can do with this demo
    .RegisterEventHandler<AppHomeOpened, AppHome>()

    // Ping demo - a simple event handler that says pong when you say ping
    .RegisterEventHandler<MessageEvent, PingDemo>()

    // Counter demo - an interactive message that updates itself
    .RegisterEventHandler<MessageEvent, CounterDemo>()
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add1)
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add5)
    .RegisterBlockActionHandler<ButtonAction, CounterDemo>(CounterDemo.Add10)

    // Modal view demo - opens a modal view with a range of different inputs
    .RegisterEventHandler<MessageEvent, ModalViewDemo>()
    .RegisterBlockActionHandler<ButtonAction, ModalViewDemo>(ModalViewDemo.OpenModal)
    .RegisterViewSubmissionHandler<ModalViewDemo>(ModalViewDemo.ModalCallbackId)

    // Echo demo - a slash command for telling you what you already know
    .RegisterSlashCommandHandler<EchoDemo>(EchoDemo.SlashCommand)
);
var services = serviceCollection.BuildServiceProvider();

Console.WriteLine("Connecting...");

var client = services.SlackServices().GetSocketModeClient();
await client.Connect();

Console.WriteLine("Connected. Press any key to exit...");
await Task.Run(Console.ReadKey);

record AppSettings
{
    public string ApiToken { get; init; } = string.Empty;
    public string AppLevelToken { get; init; } = string.Empty;
}