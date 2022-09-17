# SlackNet Demo
An example of some of the things you can do with a Slack app written with the help of SlackNet. It uses SlackNet's `SlackSocketModeClient` to connect to Slack as easily as possible.

This project makes use of `Microsoft.Extensions.DependencyInjection` and `SlackNet.Extensions.DependencyInjection` for configuring SlackNet.
This is the same DI container that ASP.NET Core uses, so configuring SlackNet in a web site will look almost identical.
Check out the [other example projects](../) to see how to configure SlackNet with other containers, or without any container.  

## Getting Started
> **TODO** This is just a very rough outline at the moment - I'll flesh it out soon.

1. Create an app on the [Slack developer website](https://api.slack.com/apps)
2. Configure socket mode
3. Configure app home
3. Subscribe to events
   - `app_home_opened` for showing app home tab
   - `message.channels` `message.groups` `message.im` `message.mpim` for receiving messages
4. Add required scopes
    - `users:read` `channels:read` `groups:read` `im:read` `mpim:read` for getting user & conversation info
    - `chat:write` for posting messages
5. Optionally configure the slash command and workflow step
6. Copy tokens into app settings
7. Run the demo

## Basic Demos
- [App Home](./AppHome.cs) - Tells you what you can do with the demo when you open the app's home view in Slack.
- [Ping](./PingDemo.cs) - A simple event handler that says pong when you say ping.
- [Counter](./CounterDemo.cs) - Displays an interactive message that updates itself.
- [Modal View](./ModalViewDemo.cs) - Opens a modal view with a range of different inputs.

## Slash Command Demo
The [echo demo](./EchoDemo.cs) handles a `/echo` slash command and sends back the text after the command name.

Follow Slack's [Creating a Slash Command](https://api.slack.com/interactivity/slash-commands#creating_commands) instructions to create the slash command in your app, and make sure the command is `/echo`, to match up with the demo code.

After this is configured and the demo is running, you should be able to type `/echo test` into Slack and receive a message saying "test".

## Workflow Step Demo
The [workflow demo](./WorkflowDemo.cs) allows you to set up a [workflow](https://api.slack.com/workflows) step that sends a predefined message to a user in Slack.

>  Workflows are only available to paid Slack workspaces.

Follow Slack's [Steps from apps](https://api.slack.com/workflows/steps#getting_started) instructions to configure the step in your Slack app,
and use the value of the `StepCallbackId` field in the [WorkflowDemo](./WorkflowDemo.cs) class as the Callback ID when you create the step.

Once the workflow step is configured in your Slack app, [set up a workflow in Slack](https://slack.com/intl/en-au/help/articles/360053571454-Set-up-a-workflow-in-Slack), choose a trigger, then add your new workflow step.

When the workflow is triggered while the demo is running, it should send your message to the specified user.

> **TODO** Needs more information - I need to actually try this end-to-end.