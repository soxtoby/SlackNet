# SlackNet Autofac Example
SlackNet services can be configured in [Autofac](https://autofac.org/) using a similar API as in other scenarios.
All Slack handlers _must_ be registered inside `AddSlackNet`, and are scoped per Slack request,
but these registrations can be overridden afterwards using the standard Autofac API.

## Getting Started
Please refer to the [Getting Started section of SlackNetDemo](../SlackNetDemo#getting-started).
This example contains only the [Ping](./PingDemo.cs) demo from that project.