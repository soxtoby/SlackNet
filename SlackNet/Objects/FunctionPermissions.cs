#nullable enable
using System.Collections.Generic;

namespace SlackNet;

/// <summary>
/// The visibility of a function, as returned by <see cref="WebApi.IAdminFunctionsPermissionsApi.Lookup"/>.
/// </summary>
public class FunctionPermissions
{
    /// <summary>
    /// Who the function has been distributed to.
    /// </summary>
    public FunctionAccess Distribution { get; set; } = new();

    /// <summary>
    /// Who an admin has allowed to use the function.
    /// </summary>
    public FunctionAccess AllowedByAdmin { get; set; } = new();

    /// <summary>
    /// Who is allowed to use the function.
    /// </summary>
    public FunctionAccess AllowedEntities { get; set; } = new();

    /// <summary>
    /// Who can use the function to access private channels.
    /// </summary>
    public EntityPermission? PrivateChannelAccess { get; set; }
}

/// <summary>
/// Who can use a function.
/// </summary>
public class FunctionAccess
{
    public FunctionPermissionType Type { get; set; }

    /// <summary>
    /// List of user IDs to allow for <see cref="FunctionPermissionType.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> UserIds { get; set; } = [];

    /// <summary>
    /// List of team IDs to allow for <see cref="FunctionPermissionType.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> TeamIds { get; set; } = [];

    /// <summary>
    /// List of org IDs to allow for <see cref="FunctionPermissionType.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> OrgIds { get; set; } = [];

    /// <summary>
    /// List of channel IDs to allow for <see cref="FunctionPermissionType.NamedEntities"/> visibility.
    /// </summary>
    public IList<string> ChannelIds { get; set; } = [];
}

/// <summary>
/// Metadata about a function, as returned by <see cref="WebApi.IAdminFunctionsPermissionsApi.Lookup"/>.
/// </summary>
public class FunctionMetadata
{
    /// <summary>
    /// The number of workflows with a step function of this type.
    /// </summary>
    public int WorkflowsUsing { get; set; }
}

/// <summary>
/// A permission to set for a function with <see cref="WebApi.IAdminFunctionsPermissionsApi.Set"/>.
/// </summary>
public class FunctionPermissionSetting
{
    public FunctionPermissionKind PermissionType { get; set; }
    public PermissionVisibility Visibility { get; set; }

    /// <summary>
    /// List of user IDs to allow for <see cref="PermissionVisibility.NamedEntities"/> visibility.
    /// </summary>
    [IgnoreIfEmpty]
    public IList<string> UserIds { get; set; } = [];
}

/// <summary>
/// What a <see cref="FunctionPermissionSetting"/> controls access to.
/// </summary>
public enum FunctionPermissionKind
{
    SlackFunction,
    PrivateChannelAccess,
    ConnectorResource
}
