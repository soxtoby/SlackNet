# ASP.NET Core Events Example

## Slack setup

Start by creating a [Slack app](https://api.slack.com/apps). 

Subscribe to the `message.*` bot events and the `app_home_opened` workspace event.  

Add the following [bot token](https://api.slack.com/authentication/token-types#granular_bot) scopes: `chat:write`, `im:write`.

Once the application is up and running, fill in the event and interactivity request URLs with https://your-app/slack/event and https://your-app/slack/action respectively.

To use the slash command example, create a slash command called `/echo` and point it at https://your-app/slack/command.

To use the workflow example, follow Slack's [Steps from apps](https://api.slack.com/workflows/steps) guide, using `test_step` as the Callback ID when you create the step for your Slack app.

## Project setup

Fill in the `ApiToken` and `SigningSecret` settings in `appsettings.json`.

The application needs to be made publicly available for Slack to be able to interact with it. When running locally, you can use a tool like [ngrok](https://ngrok.com/) get a public URL.

Alternatively, you can switch to socket mode by following the instructions in the [socket mode example](../SlackNet.SocketModeExample/readme.md), and passing `true` into `UseSocketMode` in `Startup.cs`.

## Running the example

Run the `SlackNet.EventsExample` project. It should open a web page with further instructions.