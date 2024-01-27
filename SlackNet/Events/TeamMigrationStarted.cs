namespace SlackNet.Events;

/// <summary>
/// Sent when a Slack team is about to be migrated between servers.
/// The websocket connection will close immediately after it is sent.
/// </summary>
public class TeamMigrationStarted : Event;