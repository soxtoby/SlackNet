# SlackNet Azure Function Example
A basic example of how to integrate with [Azure Functions](https://azure.microsoft.com/en-au/services/functions).

## Getting Started
1. Follow the [Azure Function instructions](https://learn.microsoft.com/en-au/azure/azure-functions/create-first-function-vs-code-csharp) to start developing Azure Functions and to host this function app in Azure.
2. Create an app on the [Slack developer website](https://api.slack.com/apps).
3. Set your function's `SlackApiToken` and `SlackSigningSecret` application settings in the Azure portal to the values provided by Slack.
4. Enable events, and set the request URL to `https://<your function's URL>/event`. Note that you'll need to have your function up and running with the signing secret configured for this to validate correctly.
5. Subscribe to the events you want to handle.
    - The included [PingDemo](./PingDemo.cs) requires `message.channels` `message.groups` `message.im` `message.mpim` for receiving messages.
6. Add the scopes that you'll need for the app.
    - For the  Ping demo, you'll want at least `chat:write` for sending messages, and `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info.
7. (Optional) Configure Interactivity & Shortcuts URLs.
    - Use `https://<your function's URL>/action` for the Interactivity Request URL.
    - Use `https://<your function's URL>/options` for the Select Menus Options Load URL.
8. (Optional) Configure Slash Commands.
    - Use `https://<your function's URL>/command` for the Request URL.
