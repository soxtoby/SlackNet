using Microsoft.Extensions.Configuration;
using NoContainerExample;
using SlackNet;
using SlackNet.Events;

Console.WriteLine("Configuring...");

var settings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build()
    .Get<AppSettings>();

var slackServices = new SlackServiceBuilder()
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(settings.ApiToken) // This gets used by the API client
    .UseAppLevelToken(settings.AppLevelToken) // This gets used by the socket mode client

    // Register your Slack handlers here
    // The context object provides access to SlackNet services as well as the request context
    .RegisterEventHandler<MessageEvent>(ctx => new PingDemo(ctx.ServiceProvider.GetApiClient()));

Console.WriteLine("Connecting...");

var client = slackServices.GetSocketModeClient();
await client.Connect();

Console.WriteLine("Connected. Press any key to exit...");
await Task.Run(Console.ReadKey);

record AppSettings(string ApiToken, string AppLevelToken);