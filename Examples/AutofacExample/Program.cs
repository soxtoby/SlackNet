using Autofac;
using AutofacExample;
using Microsoft.Extensions.Configuration;
using SlackNet.Autofac;
using SlackNet.Events;

Console.WriteLine("Configuring...");

var settings = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build()
    .Get<AppSettings>();

var containerBuilder = new ContainerBuilder();
containerBuilder.AddSlackNet(c => c
    // Configure the tokens used to authenticate with Slack
    .UseApiToken(settings.ApiToken) // This gets used by the API client
    .UseAppLevelToken(settings.AppLevelToken) // This gets used by the socket mode client

    // Register your Slack handlers here
    .RegisterEventHandler<MessageEvent, PingDemo>()
);
await using var container = containerBuilder.Build();

Console.WriteLine("Connecting...");

var client = container.SlackServices().GetSocketModeClient(); // .SlackServices() is a convenience method for resolving SlackNet services, but you can also resolve ISlackSocketModeClient directly
await client.Connect();

Console.WriteLine("Connected. Press any key to exit...");
await Task.Run(Console.ReadKey);

record AppSettings
{
    public string ApiToken { get; init; } = string.Empty;
    public string AppLevelToken { get; init; } = string.Empty;
}