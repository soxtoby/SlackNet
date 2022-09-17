# SlackNet ASP.NET Core Example
A basic example of how to integrate with ASP.NET Core.

## Getting Started
1. Host the web site publicly, or enable socket mode for local testing.
2. Create an app on the [Slack developer website](https://api.slack.com/apps).
3. Fill in the Slack tokens in `appsettings.json` with the values provided by Slack.
4. Enable events, and set the request URL to `https://<your site's base URL>/slack/event`. Note that you'll need to have your web site up and running with the signing secret configured in `appsettings.json` for this to validate correctly.
5. Subscribe to the events you want to handle.
   - The included [PingDemo](./PingDemo.cs) requires `message.channels` `message.groups` `message.im` `message.mpim` for receiving messages.
6. Add the scopes that you'll need for the app.
   - For the  Ping demo, you'll want at least `chat:write` for sending messages, and `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info. 
7. (Optional) Configure Interactivity & Shortcuts URLs.
   - Use `https://<your site's base URL>/slack/action` for the Interactivity Request URL.
   - Use `https://<your site's base URL>/slack/options` for the Select Menus Options Load URL.
8. (Optional) Configure Slash Commands.
   - Use `https://<your site's base URL>/slack/command` for the Request URL.

## Hosting
For Slack to be able to send your web site requests, it must be hosted with a publicly accessible HTTPS address.
Some popular hosting services for ASP.NET apps include [Azure](https://azure.microsoft.com/en-au/services/app-service/web/), [AWS](https://aws.amazon.com/elasticbeanstalk), and [Google Cloud](https://cloud.google.com/appengine).

Justin Gerber has written [a good blog post](https://medium.com/@justinshawngerber/how-to-create-a-simple-slackbot-using-c-and-net-6-ebfec7692f41) that walks you through setting up a Slack app and hosting it in AWS.