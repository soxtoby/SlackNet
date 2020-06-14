# SlackNet
An easy-to-use and flexible API for writing Slack bots in .NET, built on top of a comprehensive Slack API client.

## Getting Started
There are three NuGet packages available to install, depending on your use case.
  - [SlackNet](https://www.nuget.org/packages/SlackNet/): A comprehensive Slack API client for .NET.
  - [SlackNet.Bot](https://www.nuget.org/packages/SlackNet.Bot/): Easy-to-use API for writing Slack bots.
  - [SlackNet.AspNetCore](https://www.nuget.org/packages/SlackNet.AspNetCore/): ASP.NET Core integration for receiving requests from Slack.

### SlackNet
To use the Web API:
```c#
var api = new SlackApiClient("<your OAuth access token here>");
```
then start calling methods:
```c#
var channels = await api.Channels.List();
```

To use the RTM API:
```c#
var rtm = new SlackRtmClient("<your OAuth access token here>");
await rtm.Connect();
rtm.Events.Subscribe(/* handle every event */);
rtm.Messages.Subscribe(/* handle message events */);
```

### SlackNet.Bot
```c#
var bot = new SlackBot("<your bot token here>");
await bot.Connect();
```
You can handle messages in a number of ways:
```c#
// .NET events
bot.OnMessage += (sender, message) => { /* handle message */ };

// Adding handlers
class MyMessageHandler: IMessageHandler { 
    public Task HandleMessage(IMessage message) {
        // handle message
    }
}
bot.AddHandler(new MyMessageHandler());

// Subscribing to Rx stream
bot.Messages.Subscribe(/* handle message */);
```
The easiest way to reply to messages is by using their `ReplyWith` method. Replies will be sent to same channel and thread as the message being replied to.
```c#
message.ReplyWith("simple text message");
message.ReplyWith(new BotMessage {
    Text = "more complicated message",
    Attachments = { new Attachment { Title = "attachments!" } }
});
message.ReplyWith(async () => {
    // Show typing indication in Slack while message is build built
    var asyncInfo = await SomeAsyncMethod();
    return new BotMessage { Text = "async message: " + asyncInfo };
});
```
SlackNet.Bot includes a simplified API for getting common information from Slack:
```c#
var user = await bot.GetUserByName("someuser");
var channels = await bot.GetChannels();
// etc.
```
Everything is cached, so go nuts getting the information you need. You can clear the cache with `bot.ClearCache()`.

### SlackNet.AspNetCore
In your Startup class:
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddSlackNet(c => c.UseApiToken("<your OAuth access token here>"));
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseSlackNet(c => c.UseSigningSecret("<your signing secret here>"));
}
```

Add event handler registrations inside the AddSlackNet callback. See the `SlackNet.EventsExample` project for more detail.

#### Azure Functions
SlackNet.AspNetCore can be used in Azure Functions as well, although it's a little more manual at the moment.

You'll need to [enable dependency injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection) in your project, then include in your Startup class:
```c#
public override void Configure(IFunctionsHostBuilder builder)
{
    builder.Services.AddSlackNet(c => c.UseApiToken("<your OAuth access token here>"));
    builder.Services.AddSingleton(new SlackEndpointConfiguration()
        .UseSigningSecret("<your signing secret here>"));
}
```

Copy [SlackEndpoint.cs](https://github.com/soxtoby/SlackNet/blob/master/SlackNet.AzureFunctionExample/SlackEndpoints.cs) into your project.
This provides the functions for Slack to call, and delegates request handling the same way the regular ASP.NET integration does, with the same methods for registering event handlers as above.

See the `SlackNet.AzureFunctionExample` and `SlackNet.EventsExample` projects for more detail.

#### Endpoints naming convention

SlackNet requires the Slack app endpoints to be named after the following convention:

**Event subscriptions**

`{route_prefix}/event`


**Interactivity**

`{route_prefix}/action`


**Select menus**

`{route_prefix}/options`


**Slash commands**

`{route_prefix}/command`


By default, the value of `{route_prefix}` is `slack`, but this can be configured like so:

```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseSlackNet(c => c.UseSigningSecret("<your signing secret here>").MapToPrefix("api/slack"));
}
```

## Contributing
Contributions are welcome. Currently, changes must be made on a feature branch, otherwise the CI build will fail.

Slack's API is large and changes often, and while their documentation is very good, it's not always 100% complete or accurate, which can easily lead to bugs or missing features in SlackNet.
Raising issues or submitting pull requests for these sorts of discrepencies is highly appreciated, as realistically I have to rely on the documentation unless I happen to be using a particular API myself.
