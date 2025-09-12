# SlackNet ASP.NET Core Example
A basic example of how to integrate with ASP.NET Core.

## Getting Started
1. [Create an app](https://docs.slack.dev/quickstart/#creating) on the Slack developer website. Follow the prompts, selecting the "from scratch" option, rather than the app manifest option.
2. [Request the bot token scopes](https://docs.slack.dev/quickstart/#scopes) required by the demo:
   - `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info.
   - `chat:write` for posting messages.
3. [Install the app to your workspace](https://docs.slack.dev/quickstart/#installing) and copy the bot user OAuth token from your app's OAuth & Permissions page into the demo's [appsettings.json](./appsettings.json) file for the value of the `ApiToken`.
4. Copy the signing secret from the Basic Information page of you app's settings to the `SigningSecret` value in [appsettings.json](./appsettings.json).
5. Host the web site publicly (see [below](#hosting)). You'll need the site up and running for the next step.
6. [Enable events](https://docs.slack.dev/apis/events-api/#subscribing) for your app, and set the request URL to `https://<your site's base URL>/slack/event`. Slack will check that your web site is up and responding to requests.
7. Subscribe to the `message.channels` `message.groups` `message.im` `message.mpim` events in order to receive messages.
8. Add your app to any channels/groups etc. you want it to respond to.
9. Say "ping" to get back a "pong".

### Configuring other URLs
These URLs aren't used by this example, but may be required for more advanced apps. 
- For [interactivity & shortcuts](https://docs.slack.dev/interactivity/handling-user-interaction/#setup):
   - Use `https://<your site's base URL>/slack/action` for the Interactivity Request URL.
   - Use `https://<your site's base URL>/slack/options` for the Select Menus Options Load URL.
- For [slash commands](https://docs.slack.dev/interactivity/implementing-slash-commands/#creating_commands):
   - Use `https://<your site's base URL>/slack/command` for the Request URL.

## Hosting
For Slack to be able to send your web site requests, it must be hosted with a publicly accessible HTTPS address.
Some popular hosting services for ASP.NET apps include [Azure](https://azure.microsoft.com/en-au/services/app-service/web/), [AWS](https://aws.amazon.com/elasticbeanstalk), and [Google Cloud](https://cloud.google.com/appengine).

Justin Gerber has written [a good blog post](https://medium.com/@justinshawngerber/how-to-create-a-simple-slackbot-using-c-and-net-6-ebfec7692f41) that walks you through setting up a Slack app and hosting it in AWS.