## SlackNet.Bot
SlackNet.Bot uses Slack's Real Time Messaging API to send and receive messages.

> For most applications, the [Socket Mode client](https://github.com/soxtoby/SlackNet#socket-mode) is a better choice: Slack has deprecated this API, and you'll need to create a classic app and use the V1 OAuth flow to use RTM.

### Getting Started
Start by adding the [SlackNet.Bot](https://www.nuget.org/packages/SlackNet.Bot/) package to your project, then connect the bot to Slack like so:

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
var conversations = await bot.GetConversations();
// etc.
```
Everything is cached, so go nuts getting the information you need. You can clear the cache with `bot.ClearCache()`.