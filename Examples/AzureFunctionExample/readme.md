# SlackNet Azure Function Example
A basic example of how to integrate with [Azure Functions](https://azure.microsoft.com/en-au/services/functions).

## Getting Started
1. [Create an app](https://api.slack.com/authentication/basics#creating) on the Slack developer website. Follow the prompts, selecting the "from scratch" option, rather than the app manifest option.
2. [Request the bot token scopes](https://api.slack.com/authentication/basics#scopes) required by the demo:
    - `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info.
    - `chat:write` for posting messages.
3. [Install the app to your workspace](https://api.slack.com/authentication/basics#installing).
4. Follow the [Azure Function instructions](https://learn.microsoft.com/en-au/azure/azure-functions/create-first-function-vs-code-csharp) to start developing Azure Functions and to host this function app in Azure.
5. Set your function's `SlackApiToken` and `SlackSigningSecret` application settings in the Azure portal to the values provided in the OAuth & Permissions and Basic Information pages of your Slack app, respectively.
6. [Enable events](https://api.slack.com/apis/connections/events-api#the-events-api__subscribing-to-event-types) for your app, and set the request URL to `https://<your function's URL>/event`. Slack will check that your function is up and responding to requests.
7. Subscribe to the `message.channels` `message.groups` `message.im` `message.mpim` events in order to receive messages.
8. Add your app to any channels/groups etc. you want it to respond to.
9. Say "ping" to get back a "pong".

### Configuring other URLs
These URLs aren't used by this example, but may be required for more advanced apps.
- For [interactivity & shortcuts](https://api.slack.com/interactivity/handling#setup):
    - Use `https://<your function's URL>/action` for the Interactivity Request URL.
    - Use `https://<your function's URL>/options` for the Select Menus Options Load URL.
- For [slash commands](https://api.slack.com/interactivity/slash-commands#creating_commands):
    - Use `https://<your function's URL>/command` for the Request URL.