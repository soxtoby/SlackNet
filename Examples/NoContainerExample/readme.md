# SlackNet No-Container Example
For most scenarios using a dependency injection container is recommended, but SlackNet provides a basic service factory for completeness.

Services can both be configured and created through the `SlackServiceBuilder`. All configuration should be performed before creating a service, as any configuration after that may not have any effect.

## Getting Started
Please refer to the [Getting Started section of SlackNetDemo](../SlackNetDemo#getting-started).
This example contains only the [Ping](./PingDemo.cs) demo from that project.