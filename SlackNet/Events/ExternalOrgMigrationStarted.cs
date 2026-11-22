#nullable disable
using System;
using Newtonsoft.Json;

namespace SlackNet.Events;

/// <summary>
/// An enterprise org migration has started on an external workspace.
/// Sent to all connections when an external workspace begins to migrate to an Enterprise organization.
/// </summary>
/// <remarks>See the <a href="https://docs.slack.dev/reference/events/external_org_migration_started/">Slack documentation</a> for more information.</remarks>
public class ExternalOrgMigrationStarted : Event
{
    /// <summary>
    /// The external workspace that is migrating.
    /// </summary>
    public Team Team { get; set; }

    /// <summary>
    /// The time the external workspace started to migrate.
    /// </summary>
    public int DateStarted { get; set; }

    /// <summary>
    /// The time the external workspace started to migrate.
    /// </summary>
    [JsonIgnore]
    public DateTime Started => DateStarted.ToDateTime().GetValueOrDefault();
}
