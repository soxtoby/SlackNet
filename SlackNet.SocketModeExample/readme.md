# Socket Mode Example

## Slack setup

Follow [Slack's socket mode instructions](https://api.slack.com/apis/connections/socket#setup) to create an app with socket mode enabled.

Your app-level token must have access to the `connections:write` scope.

You'll also need a [bot token](https://api.slack.com/authentication/token-types#granular_bot) with access to the `channels:read`, `chat:write`, and `users:read` scopes.

## Project setup

Fill in the `ApiToken` and `AppLevelToken` settings in `appsettings.json`.

## Running the example

Run the `SlackNet.SocketModeExample` project. It will log to the console when it has connected.

Invite your Slack app into a channel, and post a message containing the word "ping"; the app will respond with "pong".