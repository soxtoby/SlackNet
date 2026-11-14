#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackNet;

/// <summary>
/// The permissions for using a trigger type, as returned by <see cref="WebApi.IAdminWorkflowsTriggersTypesPermissionsApi.Lookup"/>.
/// Depending on a toggle, Slack returns either <see cref="Permission"/> with the entities it applies to,
/// or granular <see cref="Permissions"/>.
/// </summary>
public class TriggerTypePermissions
{
    /// <summary>
    /// Who can use the trigger type. Not included when granular <see cref="Permissions"/> are returned instead.
    /// </summary>
    public PermissionVisibility? Permission { get; set; }

    /// <summary>
    /// List of user IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// List of team IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> TeamIds { get; set; } = [];

    /// <summary>
    /// List of org IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> OrgIds { get; set; } = [];

    /// <summary>
    /// Permissions for the trigger type, by permission type. Null unless granular permissions are enabled.
    /// </summary>
    public IList<TriggerTypePermission>? Permissions { get; set; }
}

/// <summary>
/// A granular permission for using a trigger type.
/// </summary>
public class TriggerTypePermission : EntityPermission
{
    public TriggerTypePermissionType Type { get; set; }
}

/// <summary>
/// Metadata about a trigger type, as returned by <see cref="WebApi.IAdminWorkflowsTriggersTypesPermissionsApi.Lookup"/>.
/// </summary>
public class TriggerTypeMetadata
{
    /// <summary>
    /// The date this trigger type was released.
    /// </summary>
    public int DateReleased { get; set; }

    /// <summary>
    /// The date this trigger type was released.
    /// </summary>
    [JsonIgnore]
    public DateTime Released => DateReleased.ToDateTime().GetValueOrDefault();

    /// <summary>
    /// The number of workflows with a trigger of this type.
    /// </summary>
    public int WorkflowsUsing { get; set; }

    /// <summary>
    /// Whether the trigger is available for use in Workflow Builder.
    /// </summary>
    public bool AvailableInWorkflowBuilder { get; set; }
}

/// <summary>
/// A permission to set for a trigger type with <see cref="WebApi.IAdminWorkflowsTriggersTypesPermissionsApi.Set"/>.
/// </summary>
public class TriggerTypePermissionSetting
{
    public TriggerTypePermissionType Type { get; set; }

    /// <summary>
    /// Who has access. <see cref="PermissionVisibility.Unset"/> is only accepted for <see cref="TriggerTypePermissionType.AgentTask"/>,
    /// and makes tasks follow the <see cref="TriggerTypePermissionType.TriggerType"/> setting.
    /// </summary>
    public PermissionVisibility Visibility { get; set; }

    /// <summary>
    /// List of user IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// List of team IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<string> TeamIds { get; set; } = [];

    /// <summary>
    /// List of org IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<string> OrgIds { get; set; } = [];
}

/// <summary>
/// What a trigger type permission controls access to.
/// </summary>
public enum TriggerTypePermissionType
{
    /// <summary>
    /// Trigger access permission.
    /// </summary>
    TriggerType,

    /// <summary>
    /// Access to private channels.
    /// </summary>
    PrivateChannelAccess,

    /// <summary>
    /// Access to text from messages in private channels.
    /// </summary>
    PrivateChannelMessageAccess,

    AgentTask
}
