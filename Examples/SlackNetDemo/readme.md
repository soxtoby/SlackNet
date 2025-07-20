# SlackNet Demo
An example of some of the things you can do with a Slack app written with the help of SlackNet. It uses SlackNet's `SlackSocketModeClient` to connect to Slack as easily as possible.

This project makes use of `Microsoft.Extensions.DependencyInjection` and `SlackNet.Extensions.DependencyInjection` for configuring SlackNet.
This is the same DI container that ASP.NET Core uses, so configuring SlackNet in a web site will look almost identical.
Check out the [other example projects](../) to see how to configure SlackNet with other containers, or without any container.  

## Getting Started
1. [Create an app](https://api.slack.com/authentication/basics#creating) on the Slack developer website. Follow the prompts, selecting the "from scratch" option, rather than the app manifest option.
2. [Request the bot token scopes](https://api.slack.com/authentication/basics#scopes) required by the demo:
   - `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info.
   - `chat:write` for posting messages.
   - `files:read` for uploading files.
3. [Install the app to your workspace](https://api.slack.com/authentication/basics#installing) and copy the bot user OAuth token from your app's OAuth & Permissions page into the demo's [appsettings.json](./appsettings.json) file for the value of the `ApiToken`.
4. [Enable socket mode](https://api.slack.com/apis/connections/socket#toggling) for your app. You'll be required to generate an app-level token - copy this into [appsettings.json](./appsettings.json) for the value of the `AppLevelToken`.
5. [Enable the home tab](https://api.slack.com/surfaces/app-home#enabling) for your app.
6. [Enable events](https://api.slack.com/apis/connections/events-api#the-events-api__subscribing-to-event-types) and subscribe to the following bot events: 
   - `app_home_opened` for showing app home tab.
   - `message.channels` `message.groups` `message.im` `message.mpim` for receiving messages.
7. Add your app to any channels/groups etc. you want it to respond to. 
8. Optionally configure the slash command (see below).
9. Run the demo.

## Basic Demos
- [App Home](./AppHome.cs) - Tells you what you can do with the demo when you open the app's home view in Slack.
- [Ping](./PingDemo.cs) - A simple event handler that says pong when you say ping.
- [Counter](./CounterDemo.cs) - Displays an interactive message that updates itself.
- [Modal View](./ModalViewDemo.cs) - Opens a modal view with a range of different inputs.

## Slash Command Demo
The [echo demo](./EchoDemo.cs) handles a `/echo` slash command and sends back the text after the command name.

Follow Slack's [Creating a Slash Command](https://api.slack.com/interactivity/slash-commands#creating_commands) instructions to create the slash command in your app, and make sure the command is set to `/echo`, to match up with the demo code.

After this is configured and the demo is running, you should be able to type `/echo test` into Slack and receive a message saying "test".
